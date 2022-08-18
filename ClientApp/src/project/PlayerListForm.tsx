import { FC, useContext } from 'react';

import { Avatar, Button, List, ListItem, ListItemAvatar, ListItemText } from '@mui/material';
import AddBoxIcon from '@mui/icons-material/AddBox';
import UploadFileIcon from '@mui/icons-material/UploadFile';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { useRest } from '../utils';

import * as DbApi from './api';
import { PlayerData } from './types';
import { PlayerContext } from './OverlayAssetsContext';
import { useNavigate } from 'react-router-dom';
import { PLAYER_IMAGE_PATH } from './projConfig';

const PlayerListForm: FC = () => {
  const navigate = useNavigate();
  const playerContext = useContext(PlayerContext);
  const {
    loadData, data, errorMessage
  } = useRest<PlayerData[]>({ read: DbApi.readPlayerList });

  const handleAdd = () => {
    console.log("Launching add page");
    navigate("../add");
  };

  const handleBulkAdd = () => {
    console.log("Launching bulk add page");
    navigate("../bulkadd");
  }


  const renderPlayer = (player: PlayerData) => {
    return (
      <ListItem
        key={player.id}
        sx={{}}
        button
        onClick={() => playerContext.selectPlayer(player)}
      >
        <ListItemAvatar>
          <Avatar src={PLAYER_IMAGE_PATH + player.image} variant="square" >
          </Avatar>
        </ListItemAvatar>
        <ListItemText
          sx={{color: 'lightgray'}}
          secondaryTypographyProps={{color: 'common.lightgray'}}
          primary={player.name}
          secondary={player.jersey + "  " + player.position + " (" + player.school + ")"}
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

    return (
      <>
        <List sx={{bgcolor: '#000000c0'}}>
            <div>
              {data.map(renderPlayer)}
            </div>
        </List>
        <ButtonRow mt={1}>
          <Button startIcon={<AddBoxIcon />} variant="contained" color="primary" type="button" onClick={handleAdd}>
            Add
          </Button>
          <Button startIcon={<UploadFileIcon />} variant="contained" color="primary" type="button" onClick={handleBulkAdd}>
            Bulk Add from .CSV
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='Players' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default PlayerListForm;
