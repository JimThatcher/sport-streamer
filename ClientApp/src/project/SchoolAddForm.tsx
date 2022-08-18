import axios from 'axios';
import { FC, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSnackbar } from 'notistack';

import { Button, TextField, Stack } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import SaveIcon from '@mui/icons-material/Save';
import UploadFileIcon from '@mui/icons-material/UploadFile';

import { SectionContent, ButtonRow } from '../components';
import { updateValue, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { SchoolData } from './types';

const SchoolAddForm: FC = () => {
  const { enqueueSnackbar } = useSnackbar();
  console.log("Loading SchoolAddForm");
  const [saving, setSaving] = useState<boolean>(false);
  const [data, setData] = useState<SchoolData>({id:0, name:"", mascot:"", color:"#000000", color2:"#000000", logo:"", win:0, loss:0});
  // const [errorMessage, setErrorMessage] = useState<string>();
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [filename, setFilename] = useState<string | undefined>(undefined);

  const goBack = () => {
    navigate("list");
  };

  const save = useCallback(async (toSave: SchoolData) => {
    setSaving(true);
    // setErrorMessage(undefined);
    try {
      await DbApi.createSchoolData(toSave);
      enqueueSnackbar("Update successful", { variant: 'success' });
    } catch (error: any) {
      const message = extractErrorMessage(error, 'Problem saving data');
      enqueueSnackbar(message, { variant: 'error' });
      // setErrorMessage(message);
    } finally {
      setSaving(false);
    }
  }, [enqueueSnackbar]);

  const updateFormValue = updateValue(setData);
  const navigate = useNavigate();

  const processSave = async () => {
    if (data) {
      console.log("processSave");
      await save(data);
      navigate('list');
    }
  };

  const doUpload = async () => {
    const fileData = new FormData();
    console.log("File is %s", uploadFile?.name);
    if (uploadFile) {
      fileData.append("File", uploadFile);
      await axios.post("./rest/db/logo/" + uploadFile.name, fileData, {
        headers: {
          'Content-Type': 'multipart/form-data',
          'Authorization': 'Bearer ' + localStorage.getItem('access_token')
        }
      }).then(function (response) {
        if (response.status === 201) {
          enqueueSnackbar("Logo upload successful", { variant: 'success' });
          setUploadFile(null);
          if (filename)
            data.logo = filename;
        }
      });
    }
  };

  const content = () => {
    return (
      <>
        <TextField
          name="name"
          label="Name"
          fullWidth
          variant="outlined"
          value={data?.name}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="mascot"
          label="Mascot"
          fullWidth
          variant="outlined"
          value={data?.mascot}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="color"
          type="color"
          label="Color"
          fullWidth
          variant="outlined"
          value={data?.color}
          onChange={updateFormValue}
          margin="normal"
        />
        <Stack direction="row" spacing="1">
          <TextField
            name="logoPrefix"
            value={filename ? filename : 'Select logo file'}
            disabled={true}
            variant='outlined'
            margin="none"
          />
          <input
            accept='image/png'
            name="logo"
            type="file"
            // value={data?.logo}
            style={{'margin':1, 'padding':15, 'outline':'solid gray 1.5px', 'borderRadius':5}}
            id="icon-button-file"
            onChange={(e) => {
              if (e && e.target && e.target.files) {
                setUploadFile(e.target.files.item(0));
                updateFormValue(e);
                setFilename(e.target.files.item(0)?.name);
              }
            }}
          >
          </input>
          <Button
            startIcon={<UploadFileIcon />}
            disabled={!uploadFile}
            variant="contained"
            color="secondary"
            type="button"
            sx={{'margin':2}}
            onClick={doUpload}
          >
              Upload
          </Button>
        </Stack>
        <TextField
          name="win"
          label="Wins this season"
          fullWidth
          variant="outlined"
          value={data?.win}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="loss"
          label="Losses this season"
          fullWidth
          variant="outlined"
          value={data?.loss}
          onChange={updateFormValue}
          margin="normal"
        />
        <ButtonRow mt={1}>
          <Button startIcon={<CancelIcon />} disabled={saving} variant="contained" color="primary" type="button" onClick={goBack}>
            Cancel
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

export default SchoolAddForm;
