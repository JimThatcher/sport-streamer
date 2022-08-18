import { FC, useContext } from 'react';

import { Avatar, Button, List, ListItem, ListItemAvatar, ListItemText } from '@mui/material';
// import { Avatar, Button, TextField, Divider, List, ListItem, ListItemAvatar, ListItemText, Theme, useTheme  } from '@mui/material';
// import SaveIcon from '@mui/icons-material/Save';
import AddBoxIcon from '@mui/icons-material/AddBox';
// import LockOpenIcon from '@mui/icons-material/LockOpen';
// import LockIcon from '@mui/icons-material/Lock';

import { SectionContent, FormLoader, ButtonRow } from '../components';
import { useRest } from '../utils';

import * as SbApi from './api';
import { SchoolData } from './types';
import { SchoolContext } from './SchoolContext';
import { useNavigate } from 'react-router-dom';

const SchoolListForm: FC = () => {
  const navigate = useNavigate();
  const schoolContext = useContext(SchoolContext);
  const {
    loadData, data, errorMessage
  } = useRest<SchoolData[]>({ read: SbApi.readSchoolList });

  // const updateFormValue = updateValue(setData);

  const handleAdd = () => {
    console.log("Launching add page");
    navigate("../add");
  };

  const renderSchool = (school: SchoolData) => {
    return (
      <ListItem
        key={school.id}
        sx={{bgcolor: school.color + "80"}}
        button
        onClick={() => schoolContext.selectSchool(school)}
      >
        <ListItemAvatar>
          <Avatar src={"./Images/" + school.logo} variant="square" >
          </Avatar>
        </ListItemAvatar>
        <ListItemText
          sx={{color: 'lightgray'}}
          secondaryTypographyProps={{color: 'common.lightgray'}}
          primary={school.name}
          secondary={school.mascot + " (Record: " + school.win + "-" + school.loss + ")"}
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
              {data.map(renderSchool)}
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
    <SectionContent title='Schools on device' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default SchoolListForm;
