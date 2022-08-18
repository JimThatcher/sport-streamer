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
import { PlayerData } from './types';

const PlayerAddForm: FC = () => {
  const { enqueueSnackbar } = useSnackbar();
  console.log("Loading PlayerAddForm");
  const [saving, setSaving] = useState<boolean>(false);
  const [data, setData] = useState<PlayerData>({id:0, name:"", jersey:0, image:"", position:"", school:"", height:0, weight:0, year:0});
  // const [errorMessage, setErrorMessage] = useState<string>();
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [filename, setFilename] = useState<string | undefined>(undefined);

  const goBack = () => {
    navigate("list");
  };

  const save = useCallback(async (toSave: PlayerData) => {
    setSaving(true);
    // setErrorMessage(undefined);
    try {
      await DbApi.createPlayerData(toSave);
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
      await axios.post("./rest/db/player/" + uploadFile.name, fileData, {
        headers: {
          'Content-Type': 'multipart/form-data',
          'Authorization': 'Bearer ' + localStorage.getItem('access_token')
        }
      }).then(function (response) {
        if (response.status === 201) {
          enqueueSnackbar("Logo upload successful", { variant: 'success' });
          setUploadFile(null);
          if (filename)
            data.image = filename;
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
          name="jersey"
          label="Jersey"
          type="number"
          fullWidth
          variant="outlined"
          value={data.jersey}
          onChange={updateFormValue}
          margin="normal"
        />
        <Stack direction="row" spacing="1">
          <TextField
            name="logoPrefix"
            value={filename ? filename : 'Player image file'}
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
          name="position"
          label="Position"
          fullWidth
          variant="outlined"
          value={data.position}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="height"
          label="Height (in inches)"
          fullWidth
          type="number"
          variant="outlined"
          value={data.height}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="weight"
          label="Weight"
          fullWidth
          type="number"
          variant="outlined"
          value={data.weight}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="year"
          label="Year (number)"
          fullWidth
          type="number"
          variant="outlined"
          value={data.year}
          onChange={updateFormValue}
          margin="normal"
        />
        <TextField
          name="school"
          label="School/Team"
          fullWidth
          variant="outlined"
          value={data.school}
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
    <SectionContent title='Player Info' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default PlayerAddForm;
