import { FC, useRef, useState, useCallback } from 'react';

import { Button, MenuItem, TextField, TextFieldProps } from '@mui/material';
import CancelIcon from '@mui/icons-material/Cancel';
import SaveIcon from '@mui/icons-material/Save';
// import { DateTimePicker } from '@material-ui/pickers';
// import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
// import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
// import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';

import { SectionContent, ButtonRow } from '../components';
import { updateValue, extractErrorMessage } from '../utils';

import * as DbApi from './api';
import { GameRecord, SchoolsList, SchoolData } from './types';
import { useNavigate } from 'react-router-dom';
import { useSnackbar } from 'notistack';
import { now } from 'lodash';

const GameAddForm: FC<SchoolsList> = ({schools}) => {
  const homeTeamField = useRef<TextFieldProps>(null);
  const awayTeamField = useRef<TextFieldProps>(null);
  const gameDateTime = useRef<TextFieldProps>(null);

  const { enqueueSnackbar } = useSnackbar();
  console.log("Loading GameAddForm");
  const [saving, setSaving] = useState<boolean>(false);
  const [date, setDate] = useState<Date | null>(new Date());
  const [data, setData] = useState<GameRecord>({id:0, homeId: schools[0].id, awayId: schools[0].id, date: getDateString(new Date())});

  const save = useCallback(async (toSave: GameRecord) => {
    setSaving(true);
    console.log("New game: home: %d, away: %d, date: %s", toSave.homeId, toSave.awayId, toSave.date);
    if (toSave.homeId === toSave.awayId)
      return;
    try {
      await DbApi.createGameData(toSave);
      enqueueSnackbar("Update successful", { variant: 'success' });
    } catch (error: any) {
      const message = extractErrorMessage(error, 'Problem saving data');
      enqueueSnackbar(message, { variant: 'error' });
    } finally {
      setSaving(false);
    }
  }, [enqueueSnackbar]);

  const updateFormValue = updateValue(setData);

  const navigate = useNavigate();

  function getDateString(date: Date): string {
    var resultStr = date.toISOString().substring(0, 10);
    return resultStr;
  }

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

  const goBack = () => {
    navigate("../list");
  };

  const processSave = async () => {
    if (data) {
      console.log("processSave");
      await save(data);
      navigate('list');
    }
  };

  const changeHomeTeam = (event: any) => {
    updateFormValue(event);
    if (event != null) {
      console.log("Home Team changed to %s", event.target.value);
      data.homeId = event.target.value as number;
    }
    console.log("Home Team changed", data);
  };

  const changeAwayTeam = (event: any) => {
    updateFormValue(event);
    if (event != null) {
      console.log("Away Team changed to %s", event.target.value);
      data.awayId = event.target.value as number;
    }
    console.log("Home Team is currently %s, UI item is %s", data.homeId, homeTeamField.current?.value || "")
  };

  const changeDate = (event: any) => {
    console.log("changeDate: %s", event.target.value);
    const date = new Date(event.target.value);
    if (date.toString() !== "Invalid Date") {
      setDate(date);
      setData({...data, date: getDateString(date)});
      console.log("Date changed", data);
    }

    /*
    setDate(event);
    if (event != null && date != null)
      data.date = date?.toISOString();
    else
      data.date = "";
    */
  }

  const content = () => {
    return (
      <>
        <TextField
          inputRef={homeTeamField}
          name="homeId"
          label="Home"
          select
          onChange={changeHomeTeam}
          value={data.homeId}
          margin="normal"
        >
          {schools.map(listSchools)}
        </TextField>
        <br />
        <TextField
          inputRef={awayTeamField}
          name="awayId"
          label="Guest"
          select
          value={data.awayId}
          onChange={(newValue) => changeAwayTeam(newValue)}
          margin="normal"
        >
          {schools.map(listSchools)}
        </TextField>
        <br />
        <TextField
          inputRef={gameDateTime}
          name="gameDate"
          label="Game Date (YYYY-MM-DD)"
          value={data.date}
          onChange={(newValue) => {
            changeDate(newValue);
          }}
        margin="normal"
        >
        </TextField>
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
    <SectionContent title='Create Game' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default GameAddForm;
