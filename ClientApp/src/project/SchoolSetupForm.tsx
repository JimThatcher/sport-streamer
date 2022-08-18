import { FC, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { Button, TextField } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import SaveIcon from '@mui/icons-material/Save';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { updateValue, useRestParam, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { SchoolData } from './types';
import { SchoolContext } from './SchoolContext';
import { useSnackbar } from 'notistack';

const SchoolSetupForm: FC = () => {
  const { selectedSchool, deselectSchool } = useContext(SchoolContext);
  const [initialized, setInitialized] = useState(false);
  var schoolId = selectedSchool ? selectedSchool.id.toString() : "1";

  const {
    loadData, saveData, saving, setData, data, errorMessage
  } = useRestParam<SchoolData>({ param: schoolId, read: DbApi.readSchoolData, update: DbApi.updateSchoolData });

  const goBack = () => {
    navigate("../list");
  };

  useEffect(() => {
    if (!initialized && data) {
      if (selectedSchool) {
        setData({
          id: selectedSchool.id,
          name: selectedSchool.name,
          mascot: selectedSchool.mascot,
          color: selectedSchool.color,
          color2: selectedSchool.color2,
          logo: selectedSchool.logo,
          win: selectedSchool.win,
          loss: selectedSchool.loss
        });
      }
      setInitialized(true);
    }
    return () => {
      // Do any needed cleanup work
    };
  }, [initialized, setInitialized, data, setData, selectedSchool]);

  const updateFormValue = updateValue(setData);
  const navigate = useNavigate();
  const { enqueueSnackbar } = useSnackbar();

  const processSave = () => {
    saveData();
    navigate('../list');
  };

  const processDelete = async () => {
    console.log("School to delete: %d", data?.id);
    if (data) {
      try {
        await DbApi.deleteSchoolRecord(data.id.toString());
        enqueueSnackbar("Delete successful", { variant: 'success' });
      } catch (error: any) {
        const message = extractErrorMessage(error, 'Problem saving data');
        enqueueSnackbar(message, { variant: 'error' });
      } finally {
      }
    }
    navigate("../list");
  };

  useEffect(() => deselectSchool, [deselectSchool]);

  const content = () => {
    if (!data) {
      if (selectedSchool) {
        setData(selectedSchool);
      }
      if (!data)
        return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }

    return (
      <>
        <TextField
          name="name"
          label="Name"
          fullWidth
          variant="outlined"
          value={data.name}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="mascot"
          label="Mascot"
          fullWidth
          variant="outlined"
          value={data.mascot}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="color"
          type="color"
          label="Color"
          fullWidth
          variant="outlined"
          value={data.color}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="logo"
          label="Logo file"
          fullWidth
          variant="outlined"
          value={data.logo}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="win"
          label="Wins this season"
          fullWidth
          variant="outlined"
          value={data.win}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="loss"
          label="Losses this season"
          fullWidth
          variant="outlined"
          value={data.loss}
          onChange={updateFormValue}
          margin="normal"
        />
        <ButtonRow mt={1}>
          <Button startIcon={<CancelIcon />} disabled={saving} variant="contained" color="primary" type="button" onClick={goBack}>
            Cancel
          </Button>
          <Button startIcon={<DeleteIcon />} disabled={saving} variant="contained" color="primary" type="button" onClick={processDelete}>
            Delete
          </Button>
          <Button startIcon={<SaveIcon />} disabled={saving} variant="contained" color="primary" type="submit" onClick={processSave}>
            Save
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='School UI Info' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default SchoolSetupForm;
