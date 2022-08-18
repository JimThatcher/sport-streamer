import { AxiosPromise } from "axios";

import { AXIOS } from "../api/endpoints";
import { GameData, GameRecord, MqttSettings, LightState, LiveClock, SchoolData, DeviceConfig, PlayerData, SponsorData } from "./types";

export function readBrokerSettings(): AxiosPromise<MqttSettings> {
  return AXIOS.get('/brokerSettings');
}

export function updateBrokerSettings(mqttSettings: MqttSettings): AxiosPromise<MqttSettings> {
  return AXIOS.post('/brokerSettings', mqttSettings);
}

export function readLiveClock(): AxiosPromise<LiveClock> {
  return AXIOS.get('/clock');
}

export function readSchoolData(param: string): AxiosPromise<SchoolData> {
  return AXIOS.get('/db/school/' + param);
}

export function readSchoolList(): AxiosPromise<SchoolData[]> {
  return AXIOS.get('/db/schools');
}

export function updateSchoolData(schoolData: SchoolData, id: string): AxiosPromise<SchoolData> {
  return AXIOS.put('/db/school/' + id, schoolData);
}

export function createSchoolData(schoolData: SchoolData): AxiosPromise<SchoolData> {
  return AXIOS.post('/db/school', schoolData);
}

export function deleteSchoolRecord(schoolId: string): AxiosPromise<void> {
  return AXIOS.delete('/db/school/' + schoolId);
}

export function readGameData(gameId: string): AxiosPromise<GameData> {
  return AXIOS.get('/db/game/' + gameId);
}

export function readGameRecord(gameId: string): AxiosPromise<GameRecord> {
  return AXIOS.get('/db/game/' + gameId + "?basic");
}

export function readGameList(): AxiosPromise<GameRecord[]> {
  return AXIOS.get('/db/games');
}

export function updateGameRecord(gameRecord: GameRecord, gameId: string): AxiosPromise<GameRecord> {
  console.log("updateGameRecord", gameRecord.id, gameRecord.homeId, gameRecord.awayId, gameRecord.date, gameId);
  return AXIOS.put('/db/game/' + gameId, gameRecord);
}

export function createGameData(gameRecord: GameRecord): AxiosPromise<GameRecord> {
  return AXIOS.post('/db/game', gameRecord);
}

export function deleteGameRecord(gameId: string): AxiosPromise<void> {
  return AXIOS.delete('/db/game/' + gameId);
}

export function deleteSchoolLogo(filename: string): AxiosPromise<void> {
  return AXIOS.delete('/db/logo/' + filename);
}

export function uploadSchoolLogo(file: File, name: string): AxiosPromise<File> {
  return AXIOS.post('db/logo/' + name, file);
}

export function readDeviceConfig(): AxiosPromise<DeviceConfig> {
  return AXIOS.get('/config');
}

export function updateDeviceConfig(deviceConfig: DeviceConfig): AxiosPromise<DeviceConfig> {
  return AXIOS.post('/config', deviceConfig);
}

export function readPlayerData(param: string): AxiosPromise<PlayerData> {
  return AXIOS.get('/db/player/' + param);
}

export function readPlayerList(): AxiosPromise<PlayerData[]> {
  return AXIOS.get('/db/players');
}

export function updatePlayerData(playerData: PlayerData, id: string): AxiosPromise<PlayerData> {
  return AXIOS.put('/db/player/' + id, playerData);
}

export function createPlayerData(playerData: PlayerData): AxiosPromise<PlayerData> {
  return AXIOS.post('/db/player', playerData);
}

export function deletePlayerData(playerId: string): AxiosPromise<void> {
  return AXIOS.delete('/db/player/' + playerId);
}

export function readSponsorData(param: string): AxiosPromise<SponsorData> {
  return AXIOS.get('/db/sponsor/' + param);
}

export function readSponsorList(): AxiosPromise<SponsorData[]> {
  return AXIOS.get('/db/sponsors');
}

export function updateSponsorData(sponsorData: SponsorData, id: string): AxiosPromise<SponsorData> {
  return AXIOS.put('/db/sponsor/' + id, sponsorData);
}

export function createSponsorData(data: SponsorData): AxiosPromise<SponsorData> {
  return AXIOS.post('/db/sponsor', data);
}

export function deleteSponsorData(id: string): AxiosPromise<void> {
  return AXIOS.delete('/db/sponsor/' + id);
}

