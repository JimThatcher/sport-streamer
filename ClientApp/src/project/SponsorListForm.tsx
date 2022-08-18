import { FC, useContext } from 'react';

import { Avatar, Button, List, ListItem, ListItemAvatar, ListItemText } from '@mui/material';
import AddBoxIcon from '@mui/icons-material/AddBox';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { useRest } from '../utils';

import * as DbApi from './api';
import { SponsorData } from './types';
import { SponsorContext } from './OverlayAssetsContext';
import { useNavigate } from 'react-router-dom';
import { SPONSOR_IMAGE_PATH } from './projConfig';

const SponsorListForm: FC = () => {
  const navigate = useNavigate();
  const sponsorContext = useContext(SponsorContext);
  const {
    loadData, data, errorMessage
  } = useRest<SponsorData[]>({ read: DbApi.readSponsorList });

  const handleAdd = () => {
    console.log("Launching add page");
    navigate("../add");
  };

  const renderSponsor = (sponsor: SponsorData) => {
    return (
      <ListItem
        key={sponsor.id}
        sx={{}}
        button
        onClick={() => sponsorContext.selectSponsor(sponsor)}
      >
        <ListItemAvatar>
          <Avatar src={SPONSOR_IMAGE_PATH + sponsor.image} variant="square" >
          </Avatar>
        </ListItemAvatar>
        <ListItemText
          sx={{color: 'lightgray'}}
          secondaryTypographyProps={{color: 'common.lightgray'}}
          primary={sponsor.name}
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
              {data.map(renderSponsor)}
            </div>
        </List>
        <ButtonRow mt={1}>
          <Button startIcon={<AddBoxIcon />} variant="contained" color="primary" type="button" onClick={handleAdd}>
            Add
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='Sponsors' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default SponsorListForm;
