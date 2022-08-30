import { FC, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { Avatar, Button, Stack, TextField } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import SaveIcon from '@mui/icons-material/Save';

import { SectionContent, FormLoader, ButtonRow, ValidatedTextField } from '../components';
import { numberValue, updateValue, useRestParam, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { PlayerData } from './types';
import { PlayerContext } from './OverlayAssetsContext';
import { useSnackbar } from 'notistack';
import { PLAYER_IMAGE_PATH } from './projConfig';
import { PLAYER_DATA_VALIDATOR } from './validators';
import { ValidateFieldsError } from 'async-validator';
import { validate } from "../validators";

const PlayerSetupForm: FC = () => {
  const { selectedPlayer, deselectPlayer } = useContext(PlayerContext);
  const [initialized, setInitialized] = useState(false);
  const [fieldErrors, setFieldErrors] = useState<ValidateFieldsError>();
  var playerId = selectedPlayer ? selectedPlayer.id.toString() : "1";

  const {
    loadData, saveData, saving, setData, data, errorMessage
  } = useRestParam<PlayerData>({ param: playerId, read: DbApi.readPlayerData, update: DbApi.updatePlayerData });

  const goBack = () => {
    navigate("../list");
  };

  useEffect(() => {
    if (!initialized && data) {
      if (selectedPlayer) {
        setData({
          id: selectedPlayer.id,
          name: selectedPlayer.name,
          jersey: selectedPlayer.jersey,
          image: selectedPlayer.image,
          position: selectedPlayer.position,
          height: selectedPlayer.height,
          weight: selectedPlayer.weight,
          year: selectedPlayer.year,
          school: selectedPlayer.school
        });
      }
      setInitialized(true);
    }
    return () => {
      // Do any needed cleanup work
    };
  }, [initialized, setInitialized, data, setData, selectedPlayer]);

  const updateFormValue = updateValue(setData);
  const navigate = useNavigate();
  const { enqueueSnackbar } = useSnackbar();

  const processSave = () => {
    saveData();
    navigate('../list');
  };

  const validateAndSubmit = async () => {
    try {
      setFieldErrors(undefined);
      if (data) {
        await validate(PLAYER_DATA_VALIDATOR(data), data);
        processSave();
      } else {
        enqueueSnackbar("No player data", { variant: "warning" });
      }
    } catch (errors: any) {
      setFieldErrors(errors);
    }
  };

  const processDelete = async () => {
    console.log("Player to delete: %d", data?.id);
    if (data) {
      try {
        await DbApi.deletePlayerData(data.id.toString());
        enqueueSnackbar("Delete successful", { variant: 'success' });
      } catch (error: any) {
        const message = extractErrorMessage(error, 'Problem deleting data');
        enqueueSnackbar(message, { variant: 'error' });
      } finally {
      }
    }
    navigate("../list");
  };

  useEffect(() => deselectPlayer, [deselectPlayer]);

  const content = () => {
    if (!data) {
      if (selectedPlayer) {
        setData(selectedPlayer);
      }
      if (!data)
        return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }

    return (
      <>
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="name"
          label="Name"
          fullWidth
          variant="outlined"
          value={data.name}
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
        <Stack direction="row" spacing={1}>
          <Avatar src={PLAYER_IMAGE_PATH + data.image} variant="square" >
          </Avatar>
          <TextField
            name="image"
            label="Player image file"
            fullWidth
            variant="outlined"
            value={data.image}
            onChange={updateFormValue}
            margin="normal"
          />
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
          label={"Height (e.g. 6'2\")"}
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
          label="Year"
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
          <Button startIcon={<DeleteIcon />} disabled={saving} variant="contained" color="primary" type="button" onClick={processDelete}>
            Delete
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

export default PlayerSetupForm;
