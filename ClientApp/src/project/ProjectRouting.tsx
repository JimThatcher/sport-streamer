import { FC } from 'react';
import { Navigate, Routes, Route } from 'react-router-dom';
import GameMain from './GameMain';
import PlayerMain from './PlayerMain';
import SponsorMain from './SponsorMain';
import { ROUTE_BASE_URL } from '../api/env';

import SBApp from './SBApp';
import SchoolMain from './SchoolMain';
import ScoreController from './ScoreController';

const ProjectRouting: FC = () => {
  return (
    <Routes>
      {
        // Add the default route for your project below
      }
      <Route path="/*" element={<Navigate to="/schlmgr" />} />
      {
        // Add your project page routes below.
      }
      <Route path="/app/*" element={<SBApp />} />
      <Route path="/control" element={<ScoreController />} />
      <Route path="/schlmgr/*" element={<SchoolMain />} />
      <Route path="/gamemgr/*" element={<GameMain />} />
      <Route path="/playermgr/*" element={<PlayerMain />} />
      <Route path="/sponsormgr/*" element={<SponsorMain />} />
    </Routes>
  );
};

export default ProjectRouting;
