import React, { FC, useCallback, useState, useEffect, useMemo, useContext } from 'react';
import { Navigate, Routes, Route, useNavigate } from 'react-router-dom';

import { useRest } from '../utils';

import * as SbApi from './api';

import { RequireAdmin, useLayoutTitle, FormLoader } from '../components';
import { GameRecord, SchoolData } from './types';
import { GameContext } from './SchoolContext';
import GameListForm from './GameListForm';
import GameSetupForm from './GameSetupForm';
import GameAddForm from './GameAddForm';
import { AuthenticatedContext } from '../contexts/authentication';

const GameMain: FC = () => {
  useLayoutTitle("Game Manager");
  const {
    loadData, data, errorMessage
  } = useRest<SchoolData[]>({ read: SbApi.readSchoolList });

  const authenticatedContext = useContext(AuthenticatedContext);
  const navigate = useNavigate();

  const [selectedGame, setSelectedGame] = useState<GameRecord>();

  const selectGame = useCallback((game: GameRecord) => {
    setSelectedGame(game);
    if (authenticatedContext.me.admin) {
      navigate('setup');
    } else {
      navigate("/scorebughd?" + game.id.toString());
    }
  }, [navigate, authenticatedContext.me.admin]);

  const deselectGame = useCallback(() => {
    setSelectedGame(undefined);
  }, []);

  const schoolMap = useMemo (() => new Map<number, SchoolData>(), []);

  useEffect(() => {
    console.log("Game Main data changed - in useEffect.");
    if (data) {
      var i = 0;
      for (i = 0; i < data?.length; i++) {
        schoolMap.set(data[i].id, data[i]);
      }
    }
  }, [data, schoolMap]);

  const content = () => {
    if (!data) {
      return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }

    return (
      <GameContext.Provider
        value={{
          selectedGame,
          selectGame,
          deselectGame,
          schoolMap
        }}
      >
        <Routes>
          <Route path="list" element={<GameListForm schools={schoolMap} />} />
          <Route
            path="setup"
            element={
              <RequireAdmin>
                <GameSetupForm schools={data} />
              </RequireAdmin>
            }
          />
          <Route
            path="add"
            element={
              <RequireAdmin>
                <GameAddForm schools={data} />
              </RequireAdmin>
            }
          />
          <Route path="/*" element={<Navigate replace to="list" />} />
        </Routes>
      </GameContext.Provider>
    );
  };

  return (
    <>
    {content()}
    </>
    );

};

export default GameMain;
