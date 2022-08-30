import { FC, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { Avatar, Button, Stack, TextField } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import SaveIcon from '@mui/icons-material/Save';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { updateValue, useRestParam, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { SponsorData } from './types';
import { SponsorContext } from './OverlayAssetsContext';
import { useSnackbar } from 'notistack';
import { PLAYER_IMAGE_PATH } from './projConfig';

const SponsorSetupForm: FC = () => {
  const { selectedSponsor, deselectSponsor } = useContext(SponsorContext);
  const [initialized, setInitialized] = useState(false);
  var sponsorId = selectedSponsor ? selectedSponsor.id.toString() : "1";

  const {
    loadData, saveData, saving, setData, data, errorMessage
  } = useRestParam<SponsorData>({ param: sponsorId, read: DbApi.readSponsorData, update: DbApi.updateSponsorData });

  const goBack = () => {
    navigate("../list");
  };

  useEffect(() => {
    if (!initialized && data) {
      if (selectedSponsor) {
        setData({
          id: selectedSponsor.id,
          name: selectedSponsor.name,
          image: selectedSponsor.image
        });
      }
      setInitialized(true);
    }
    return () => {
      // Do any needed cleanup work
    };
  }, [initialized, setInitialized, data, setData, selectedSponsor]);

  const updateFormValue = updateValue(setData);
  const navigate = useNavigate();
  const { enqueueSnackbar } = useSnackbar();

  const processSave = () => {
    saveData();
    navigate('../list');
  };

  const processDelete = async () => {
    console.log("Sponsor to delete: %d", data?.id);
    if (data) {
      try {
        await DbApi.deleteSponsorData(data.id.toString());
        enqueueSnackbar("Delete successful", { variant: 'success' });
      } catch (error: any) {
        const message = extractErrorMessage(error, 'Problem deleting data');
        enqueueSnackbar(message, { variant: 'error' });
      } finally {
      }
    }
    navigate("../list");
  };

  useEffect(() => deselectSponsor, [deselectSponsor]);

  const content = () => {
    if (!data) {
      if (selectedSponsor) {
        setData(selectedSponsor);
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
        <Stack direction="row" spacing={2}>
          <Avatar src={PLAYER_IMAGE_PATH + data.image} variant="square" >
          </Avatar>
          <TextField
            name="image"
            label="Sponsor image file"
            fullWidth
            variant="outlined"
            value={data.image}
            onChange={updateFormValue}
            margin="normal"
          />
        </Stack>
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
    <SectionContent title='Sponsor Info' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default SponsorSetupForm;
