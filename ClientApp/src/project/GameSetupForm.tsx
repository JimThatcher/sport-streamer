import { FC, useContext, useEffect, useRef, useState, useCallback } from 'react';

import { Button, MenuItem, TextField, TextFieldProps } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import SaveIcon from '@mui/icons-material/Save';
import AddBoxIcon from '@mui/icons-material/AddBox';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { updateValue, useRestParam, extractErrorMessage } from '../utils';
import { AxiosPromise } from "axios";

import * as DbApi from './api';
import { GameRecord, SchoolsList, SchoolData } from './types';
import { GameContext } from './SchoolContext';
import { useNavigate } from 'react-router-dom';
import { useSnackbar } from 'notistack';

import { DatePicker } from '@mui/x-date-pickers';

const GameSetupForm: FC<SchoolsList> = ({schools}) => {
  const { selectedGame, selectGame } = useContext(GameContext);
  const [initialized, setInitialized] = useState(false);
  const [homeTeamId, setHomeId] = useState(selectedGame?.homeId);
  const [awayTeamId, setAwayId] = useState(selectedGame?.awayId);
  const [date, setDate] = useState<string | null>(selectedGame?.date || getDateString(new Date()));
  const homeTeamField = useRef<TextFieldProps>(null);
  const awayTeamField = useRef<TextFieldProps>(null);
  const gameDateTime = useRef(null);
  var gameId = selectedGame?.id.toString() || "1";

/*
  const {
    loadData, saveData, saving, setData, data, errorMessage
  } = useRestParam<GameRecord>({ param: gameId, read: DbApi.readGameRecord, update: DbApi.updateGameRecord });
*/
const { enqueueSnackbar } = useSnackbar();

const navigate = useNavigate();

const [saving, setSaving] = useState<boolean>(false);
// const [thisGame, setData] = useState<GameRecord>();
// const [game, setGame] = useState<GameRecord>();
const [errorMessage, setErrorMessage] = useState<string>();
console.log("GameSetupForm rendering");

const loadData = useCallback(async () => {
  setErrorMessage(undefined);
  if (!selectedGame) {
    try {
      selectGame((await DbApi.readGameRecord(gameId)).data);
      console.log("Game data:", selectedGame);
    } catch (error: any) {
      const message = extractErrorMessage(error, 'Problem loading data');
      enqueueSnackbar(message, { variant: 'error' });
      setErrorMessage(message);
    }
  }
}, [enqueueSnackbar, gameId]);

const save = useCallback(async (toSave: GameRecord) => {
  if (!DbApi.updateGameRecord) {
    return;
  }
  setSaving(true);
  setErrorMessage(undefined);
  try {
    console.log("Saving game: ", toSave);
    const gameDate: Date = new Date(toSave.date);
    if (gameDate.toString() != "Invalid Date") 
      toSave.date = getDateString(gameDate);
    console.log("Modified Date: ", toSave.date);
    selectGame((await DbApi.updateGameRecord(toSave, gameId)).data);
    console.log("After update, game data:", selectedGame);
    enqueueSnackbar("Update successful", { variant: 'success' });
  } catch (error: any) {
    const message = extractErrorMessage(error, 'Problem saving data');
    enqueueSnackbar(message, { variant: 'error' });
    setErrorMessage(message);
  } finally {
    setSaving(false);
  }
}, [enqueueSnackbar, gameId]);

const saveData = () => selectedGame && save(selectedGame);

useEffect(() => {
  loadData();
}, [loadData]);

  useEffect(() => {
    if (!initialized && selectedGame) {
  /*    if (selectedGame) {
        setThisGame(selectedGame);
      } */
      setInitialized(true);
    }
  }, [initialized, setInitialized, selectedGame]);

  const listSchools = (school: SchoolData) => {
    return (
      <MenuItem
        key={school.id}
        value={school.id}
      >
        {school.name + " " + school.mascot}
      </MenuItem>
    );
  };

  function getDateString(date: Date): string {
    var resultStr = date.toISOString().substring(0, 10);
    return resultStr;
  }

  const goBack = () => {
    navigate("../list");
  };

  const processSave = async () => {
    var game: GameRecord = {id: selectedGame?.id || 0, 
      homeId: homeTeamId || 0, 
      awayId: awayTeamId || 0, 
      date: (date && new Date(date).toString() != 'Invalid Date') ? getDateString(new Date(date)) : getDateString(new Date())};
    console.log("Updating game.", selectedGame, game);
    await save(game);
    navigate("../list");
  };

  const addSchool = () => {
    navigate("../../schlmgr/add");
  };

  const processDelete = async () => {
    console.log("game to delete: %d", selectedGame?.id);
    if (selectedGame) {
      try {
        await DbApi.deleteGameRecord(selectedGame.id.toString());
        enqueueSnackbar("Delete successful", { variant: 'success' });
      } catch (error: any) {
        const message = extractErrorMessage(error, 'Problem saving data');
        enqueueSnackbar(message, { variant: 'error' });
      } finally {
      }
    }
    navigate("../list");
  };

  const changeHomeTeam = (event: any) => {
    updateValue(event);
    if (event != null) {
      console.log("Home Team changed to %s", event.target.value);
      setHomeId(event.target.value);
    }
  };

  const changeAwayTeam = (event: any) => {
    updateValue(event);
    if (event != null) {
      console.log("Away Team changed to %s", event.target.value);
      setAwayId(event.target.value);
    }
  };

  const changeDate = (event: any) => {
    // updateValue(event);
    console.log("changeDate: %s", event);
    const newDate: Date = new Date(event.target.value);
    if (newDate.toString() != "Invalid Date") {
      setDate(getDateString(newDate));
      if (date) {
        console.log("date set to %s", date.toString());
      }  
    }
  }

  const content = () => {
    if (!selectedGame) {
      return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }

    return (
      <>
        <TextField
          name="gameId"
          label="Game ID"
          value={selectedGame.id}
          disabled
          margin='normal'
          fullWidth
          onClick={() => {navigate("/scorebughd?" + selectedGame.id.toString());}}
        >
        </TextField>
        <TextField
          inputRef={homeTeamField}
          name="homeId"
          label="Home"
          fullWidth
          select
          onChange={changeHomeTeam}
          // variant="standard"
          value={homeTeamId}
          margin="normal"
        >
          {schools.map(listSchools)}
        </TextField>
        <TextField
          inputRef={awayTeamField}
          name="awayId"
          label="Guest"
          fullWidth
          select
          value={awayTeamId}
          onChange={changeAwayTeam}
          margin="normal"
        >
          {schools.map(listSchools)}
        </TextField>
        <TextField
          inputRef={gameDateTime}
          name="gameDate"
          label="Game Date [DO NOT USE]"
          value={date}
          onChange={(newValue) => {
            changeDate(newValue);
          }}
          margin="normal"
        >
        </TextField>
        <DatePicker 
          label="Game Date"
          value={date}
          onChange={(newValue) => {
            changeDate(newValue);
          }}
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
          <Button disabled hidden />
          <Button startIcon={<AddBoxIcon />} variant="contained" color="secondary" type="button" onClick={addSchool}>
            Add School
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='Game Info' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default GameSetupForm;
