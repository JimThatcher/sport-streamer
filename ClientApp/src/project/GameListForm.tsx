import { FC, useContext } from 'react';

import { Button, List, ListItem, ListItemText } from '@mui/material';
// import { Avatar, Button, TextField, Divider, List, ListItem, ListItemAvatar, ListItemText, Theme, useTheme  } from '@mui/material';
import AddBoxIcon from '@mui/icons-material/AddBox';
// import LockOpenIcon from '@mui/icons-material/LockOpen';
// import LockIcon from '@mui/icons-material/Lock';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { useRest } from '../utils';

import * as DbApi from './api';
import { GameRecord, SchoolMap } from './types';
import { GameContext } from './SchoolContext';
import { AuthenticatedContext } from '../contexts/authentication';
import { useNavigate, Link } from 'react-router-dom';
import { PROJECT_PATH } from '../api/env';

const GameListForm: FC<SchoolMap> = ({schools}) => {
  const authenticatedContext = useContext(AuthenticatedContext);
  const gameContext = useContext(GameContext);
  // var schlList = new Map<number, string>();
  const navigate = useNavigate();
  const {
    loadData, saving, data, errorMessage
  } = useRest<GameRecord[]>({ read: DbApi.readGameList });

  const handleAdd = () => {
    console.log("Launching add page");
    navigate("../add");
  };

  const renderGame = (game: GameRecord) => {
    return (
      <ListItem
        key={game.id}
        button
        onClick={() => { if (authenticatedContext.me.admin) gameContext.selectGame(game); }}
      >
        <ListItemText
          sx={{color: 'black'}}
          primary={(schools.get(game.homeId)?.name || game.homeId) + " vs." + (schools.get(game.awayId)?.name || game.awayId)}
          secondary={ authenticatedContext.me.admin ? "Tap/click to edit" : ""}
        />
      </ListItem>
    );
  };

  const content = () => {
    if (!data) {
      return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }
    if (!Array.isArray(data)) {
        return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }
    if (schools.size < 2) {
      return (
        <>
          <h1>Not enough schools!</h1>
          <p>You need at least two schools in the database bafore you can add a game.
            Go to <Link to={`/schlmgr/`}>Schools</Link> to add schools.
          </p>
        </>
      );
    }

    return (
      <>
        <List>
            <div>
              {data.map(renderGame)}
            </div>
        </List>
        <ButtonRow mt={1}>
          <Button startIcon={<AddBoxIcon />} disabled={saving} variant="contained" color="primary" type="button" onClick={handleAdd}>
            Add
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='Games on device' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default GameListForm;
