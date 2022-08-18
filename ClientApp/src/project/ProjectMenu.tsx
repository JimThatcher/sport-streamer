import { FC } from 'react';

import { List } from '@mui/material';
import SettingsRemoteIcon from '@mui/icons-material/SettingsRemote';
import SchoolIcon from '@mui/icons-material/School';
import AccountTreeIcon from '@mui/icons-material/AccountTree';
import KeyboardAltIcon from '@mui/icons-material/KeyboardAlt';
import PersonIcon from '@mui/icons-material/Person';
import LocalAtmIcon from '@mui/icons-material/LocalAtm';

import { PROJECT_PATH } from '../api/env';
import LayoutMenuItem from '../components/layout/LayoutMenuItem';

const ProjectMenu: FC = () => (
  <List>
    <LayoutMenuItem icon={SchoolIcon} label="Schools" to={`/schlmgr/`} />
    <LayoutMenuItem icon={AccountTreeIcon} label="Games" to={`/gamemgr/`} />
    <LayoutMenuItem icon={PersonIcon} label="Players" to={`/playermgr/`} />
    <LayoutMenuItem icon={LocalAtmIcon} label="Sponsors" to={`/sponsormgr/`} />
    <LayoutMenuItem icon={KeyboardAltIcon} label="Controller" to={`/control/`} />
  </List>
);

export default ProjectMenu;
