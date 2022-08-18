import React from 'react';

import { PlayerData, SponsorData } from './types';

export interface PlayerContextValue {
  selectedPlayer?: PlayerData;
  selectPlayer: (school: PlayerData) => void;
  deselectPlayer: () => void;
}

export interface SponsorContextValue {
  selectedSponsor?: SponsorData;
  selectSponsor: (game: SponsorData) => void;
  deselectSponsor: () => void;
}

const PlayerContextDefaultValue = {} as PlayerContextValue;
export const PlayerContext = React.createContext(
    PlayerContextDefaultValue
);

const SponsorContextDefaultValue = {} as SponsorContextValue;
export const SponsorContext = React.createContext(
  SponsorContextDefaultValue
);
