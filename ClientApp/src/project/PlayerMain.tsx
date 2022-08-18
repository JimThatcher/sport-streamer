import React, { FC, useCallback, useState } from 'react';
import { Navigate, Routes, Route, useNavigate } from 'react-router-dom';

import { RequireAdmin, useLayoutTitle } from '../components';
import { PlayerData } from './types';
// import { AuthenticatedContext } from '../contexts/authentication';
import { PlayerContext } from './OverlayAssetsContext';
import PlayerListForm from './PlayerListForm';
import PlayerSetupForm from './PlayerSetupForm';
import PlayerAddForm from './PlayerAddForm';

const PlayerMain: FC = () => {
  useLayoutTitle("Player Manager");

  // const authenticatedContext = useContext(AuthenticatedContext);
  const navigate = useNavigate();

  const [selectedPlayer, setSelectedPlayer] = useState<PlayerData>();

  const selectPlayer = useCallback((school: PlayerData) => {
    setSelectedPlayer(school);
    navigate('setup');
  }, [navigate]);

  const deselectPlayer = useCallback(() => {
    setSelectedPlayer(undefined);
  }, []);

  return (
    <PlayerContext.Provider
      value={{
        selectedPlayer: selectedPlayer,
        selectPlayer: selectPlayer,
        deselectPlayer: deselectPlayer
      }}
    >
      <Routes>
        <Route path="list" element={<PlayerListForm />} />
        <Route
          path="setup"
          element={
            <RequireAdmin>
              <PlayerSetupForm />
            </RequireAdmin>
          }
        />
        <Route
          path="add"
          element={
            <RequireAdmin>
              <PlayerAddForm />
            </RequireAdmin>
          }
        />
        <Route path="/*" element={<Navigate replace to="list" />} />
      </Routes>
    </PlayerContext.Provider>
  );

};

export default PlayerMain;
