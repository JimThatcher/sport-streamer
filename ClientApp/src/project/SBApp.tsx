
import React, { FC, useContext } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { FeaturesContext } from '../contexts/features';

import { Tab } from '@mui/material';

import { RouterTabs, useRouterTab, useLayoutTitle } from '../components';

import SchoolSetupForm from './SchoolSetupForm';
import SchoolListForm from './SchoolListForm';
import SchoolMain from './SchoolMain';
import ScoreCanvas from './ScoreCanvas';
import DeviceConfigForm from './DeviceConfig';
import ScoreController from './ScoreController';
import PlayerMain from './PlayerMain';
import PlayerListForm from './PlayerListForm';
import PlayerSetupForm from './PlayerSetupForm';
import SponsorMain from './SponsorMain';
import SponsorListForm from './SponsorListForm';
import SponsorSetupForm from './SponsorSetupForm';
import { ROUTE_BASE_URL } from '../api/env';

const SBApp: FC = () => {
  useLayoutTitle("Scoreboard Manager");
  const { routerTab } = useRouterTab();
  const { features } = useContext(FeaturesContext);

  return (
    <>
      <RouterTabs value={routerTab}>
        {features.device && (
          <Tab value="config" label="Device Configuration" />
        )}
        {features.device && (
          <Tab value="rest" label="REST Example" />
        )}
        <Tab value="score" label="Scoreboard" />
      </RouterTabs>
      <Routes>
        <Route path="/schlmgr/*" element={<SchoolMain />} />
        <Route path="/playerMgr/*" element={<PlayerMain />} />
        <Route path="/sponsorMgr/*" element={<SponsorMain />} />
        <Route path="/control" element={<ScoreController />} />
        <Route path="/score" element={<ScoreCanvas />} />
        {features.device && (
          <Route path="config" element={<DeviceConfigForm />} />
        )}
        <Route path="/schools" element={<SchoolListForm />} />
        <Route path="/school" element={<SchoolSetupForm />} />
        <Route path="/players" element={<PlayerListForm />} />
        <Route path="/player" element={<PlayerSetupForm />} />
        <Route path="/sponsors" element={<SponsorListForm />} />
        <Route path="/sponsor" element={<SponsorSetupForm />} />
        <Route path="/*" element={<Navigate replace to="/schools" />} />
      </Routes>
    </>
  );
};

export default SBApp;
