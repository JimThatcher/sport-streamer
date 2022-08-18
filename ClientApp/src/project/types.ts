export interface LightState {
  led_on: boolean;
}

export interface MqttSettings {
  unique_id: string;
  name: string;
  mqtt_path: string;
}

export interface LiveClock {
  clock: string;
  playClock: string;
  timeoutClock: string;
}

export interface SchoolData {
  id: number;
  name: string;
  mascot: string;
  color: string;
  color2: string;
  logo: string;
  win: number;
  loss: number;
}

export interface SchoolsList {
  schools: SchoolData[];
}

export interface SchoolMap {
  schools: Map<number, SchoolData>;
}

export interface GameRecord {
  id: number;
  homeId: number;
  awayId: number;
  date: string
}

export interface GameData {
  id: number;
  gameDate: string;
  HomeName: string;
  HomeColor: string;
  HomeIcon: string;
  HomeRecord: string;
  GuestName: string;
  GuestColor: string;
  GuestIcon: string;
  GuestRecord: string;
}

export enum ScoreDataSource {
  SRC_RS232 = 0,
  SRS_RADIO = 1,
  SRC_WEB = 2,
  SRC_MQTT = 3
}

export enum ScoreDataFormat {
  FORMAT_RTD = 0,
  FORMAT_JSON = 1
}

export interface DeviceConfig {
  data_src: ScoreDataSource;
  data_fmt: ScoreDataFormat;
  radio_channel: number;
  radio_address: number;
  radio_encrypt: number;
  web_addr: string;
  web_port: number;
  mqtt_enabled: boolean;
  output_to_radio: boolean;
}

export interface PlayerData {
  id: number;
  name: string;
  jersey: number;
  image: string;
  position: string;
  height: number;
  weight: number;
  year: number;
  school: string;
}

export interface PlayerList {
  players: PlayerData[];
}

export interface SponsorData {
  id: number;
  name: string;
  image: string;
}

export interface SponsorList {
  sponsors: SponsorData[];
}