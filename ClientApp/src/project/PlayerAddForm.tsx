import axios from 'axios';
import { FC, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSnackbar } from 'notistack';

import { Button, TextField, Stack } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import SaveIcon from '@mui/icons-material/Save';
import UploadFileIcon from '@mui/icons-material/UploadFile';

import { SectionContent, ButtonRow, ValidatedTextField } from '../components';
import { numberValue, updateValue, useRestParam, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { PlayerData } from './types';
import { PLAYER_DATA_VALIDATOR } from './validators';
import { ValidateFieldsError } from 'async-validator';
import { validate } from "../validators";


const PlayerAddForm: FC = () => {
  const { enqueueSnackbar } = useSnackbar();
  console.log("Loading PlayerAddForm");
  const [saving, setSaving] = useState<boolean>(false);
  const [data, setData] = useState<PlayerData>({id:0, name:"", jersey:0, image:"", position:"", school:"", height:"", weight:0, year:0});
  // const [errorMessage, setErrorMessage] = useState<string>();
  const [fieldErrors, setFieldErrors] = useState<ValidateFieldsError>();
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

  const validateAndSubmit = async () => {
    try {
      setFieldErrors(undefined);
      await validate(PLAYER_DATA_VALIDATOR(data), data);
      processSave();
    } catch (errors: any) {
      setFieldErrors(errors);
      console.log("errors", errors);
      // enqueueSnackbar(errors, { variant: "warning" });
    }
  };

  const doUpload = async () => {
    const fileData = new FormData();
    console.log("File is %s", uploadFile?.name);
    if (uploadFile) {
      fileData.append("image", uploadFile);
      await axios.post("./rest/db/player/" + uploadFile.name, fileData, {
        headers: {
          'Content-Type': 'multipart/form-data',
          'Authorization': 'Bearer ' + localStorage.getItem('access_token')
        }
      }).then(function (response) {
        if (response.status === 201) {
          enqueueSnackbar("Player image upload successful", { variant: 'success' });
          setUploadFile(null);
          if (filename)
            data.image = filename;
        }
      })
      .catch(function (err) {
        enqueueSnackbar("Player image upload failed", { variant: 'error' });
      })
    }
  };

  const content = () => {
    return (
      <>
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="name"
          label="Name"
          fullWidth
          variant="outlined"
          value={data?.name}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="jersey"
          label="Jersey number (0-199)"
          fullWidth
          variant="outlined"
          value={numberValue(data.jersey)}
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
                // TODO: If file is a PNG, and file size is less than 1MB, then upload file.
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
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="position"
          label="Position"
          fullWidth
          variant="outlined"
          value={data.position}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="height"
          label="Height (in inches)"
          fullWidth
          variant="outlined"
          value={data.height}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="weight"
          label="Weight"
          fullWidth
          variant="outlined"
          value={numberValue(data.weight)}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="year"
          label="Year (number)"
          fullWidth
          variant="outlined"
          value={numberValue(data.year)}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
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
          <Button startIcon={<SaveIcon />} disabled={saving} variant="contained" color="primary" type="submit" onClick={validateAndSubmit}>
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
