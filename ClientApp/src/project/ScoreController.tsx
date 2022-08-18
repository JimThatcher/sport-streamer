import { FC } from 'react';

import { Typography, Box, List, ListItem, ListItemText, Theme, Grid, Button } from '@mui/material';
import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import { SectionContent } from '../components';
import { red, green } from '@mui/material/colors';

const Item = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
  ...theme.typography.body2,
  padding: theme.spacing(1),
  textAlign: 'center',
  color: theme.palette.text.secondary,
  height: '60px'
}));

const HomeItem = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
  ...theme.typography.body2,
  padding: theme.spacing(1),
  textAlign: 'center',
  color: green[500],
  height: '60px'
}));

const GuestItem = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
  ...theme.typography.body2,
  padding: theme.spacing(1),
  textAlign: 'center',
  color: red[500],
  height: '60px'
}));

const sendActionCmd: Function = (action: string) => {
  // Send a GET request to the server to execute the action
  fetch(action) 
  .then(response => response.json())
  .then(data => {
    console.log(data);
  }).catch(err => {
    console.log(err);
  });
}

const ScoreController: FC = () => (
  <SectionContent title='Controller' titleGutter>
    <Grid container spacing={2} columns={14}>
      { 
        // First Row of Buttons 
      }
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/homescore/1'); }} fullWidth>Home<br/>+1</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/homescore/-1'); }} fullWidth>Home<br/>-1</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/clock/Start'); }} fullWidth>Start</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/clock/adjust/2'); }} fullWidth>Time<br/>+2</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/clock/adjust/-2'); }} fullWidth>Time<br/>-2</Item>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guestscore/1'); }} fullWidth>Guest<br/>+1</GuestItem>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guestscore/-1'); }} fullWidth>Guest<br/>-1</GuestItem>
      </Grid>
      { 
        // Second Row of Buttons 
      }
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/homescore/2'); }} fullWidth>Home<br/>+2</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { 
          sendActionCmd('/api/score/hometol/Down');
          sendActionCmd('/api/score/timeout/Home');
           }} fullWidth>Time<br/>&lt;</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/clock/Stop'); }} fullWidth>Stop</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/quarter/Up'); }} fullWidth>Quarter<br/>+1</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/togo/10'); }} fullWidth>To Go<br/>+10</Item>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guestscore/2'); }} fullWidth>Guest<br/>+2</GuestItem>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { 
          sendActionCmd('/api/score/guesttol/Down');
          sendActionCmd('/api/score/timeout/Guest');
           }} fullWidth>Time<br/>&gt;</GuestItem>
      </Grid>
      { 
        // Third Row of Buttons 
      }
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/homescore/3'); }} fullWidth>Home<br/>+3</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/hometol/Reset'); }} fullWidth>Time<br/>=3</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/togo/-5'); }} fullWidth>To Go<br/>-5</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/down/Up'); }} fullWidth>Down<br/>+1</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/togo/5'); }} fullWidth>To Go<br/>+5</Item>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guestscore/3'); }} fullWidth>Guest<br/>+3</GuestItem>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guesttol/Reset'); }} fullWidth>Time<br/>=3</GuestItem>
      </Grid>
      { 
        // Fourth Row of Buttons
      }
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/homescore/6'); }} fullWidth>Home<br/>+6</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <HomeItem variant='outlined' onClick={() => { sendActionCmd('/api/score/changeposs/Home'); }} fullWidth>Poss<br/>&lt;</HomeItem>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/togo/-1'); }} fullWidth>To Go<br/>-1</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/down/Reset'); }} fullWidth>First<br/>Down</Item>
      </Grid>
      <Grid item xs={2}>
        <Item variant='outlined' onClick={() => { sendActionCmd('/api/score/togo/1'); }} fullWidth>To Go<br/>+1</Item>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/guestscore/6'); }} fullWidth>Guest<br/>+6</GuestItem>
      </Grid>
      <Grid item xs={2}>
        <GuestItem variant='outlined' onClick={() => { sendActionCmd('/api/score/changeposs/Guest'); }} fullWidth>Poss<br/>&gt;</GuestItem>
      </Grid>
    </Grid>
  </SectionContent>
);

export default ScoreController;
