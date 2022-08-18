import React from 'react';

import { SchoolData, GameRecord } from './types';

export interface SchoolContextValue {
  selectedSchool?: SchoolData;
  selectSchool: (school: SchoolData) => void;
  deselectSchool: () => void;
}

export interface GameContextValue {
  selectedGame?: GameRecord;
  selectGame: (game: GameRecord) => void;
  deselectGame: () => void;
  schoolMap: Map<number, SchoolData>;
}

const SchoolContextDefaultValue = {} as SchoolContextValue;
export const SchoolContext = React.createContext(
    SchoolContextDefaultValue
);

const GameContextDefaultValue = {} as GameContextValue;
export const GameContext = React.createContext(
  GameContextDefaultValue
);
