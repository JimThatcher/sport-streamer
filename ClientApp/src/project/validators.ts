import Schema, { InternalRuleItem, ValidateOption } from "async-validator";
import { DeviceConfig, ScoreDataSource, PlayerData } from "./types";
import { IP_OR_HOSTNAME_VALIDATOR } from "../validators/shared";

export const LIGHT_MQTT_SETTINGS_VALIDATOR = new Schema({
    unique_id: {
        required: true, message: "Please provide an id"
    },
    name: {
        required: true, message: "Please provide a name"
    },
    mqtt_path: {
        required: true, message: "Please provide an MQTT path"
    }
});

const HEIGHT_REGEXP = /^\d' ?(?:\d|1[0-1])"?$/;
const isValidHeight = (value: string) => HEIGHT_REGEXP.test(value);

export const HEIGHT_VALIDATOR = {
  validator(rule: InternalRuleItem, value: string, callback: (error?: string) => void) {
    if (value && !isValidHeight(value)) {
      callback("Enter feet and inches (e.g. 5'11\")");
    } else {
      callback();
    }
  }
};

export const PLAYER_DATA_VALIDATOR = (player: PlayerData | undefined) => new Schema({
    name: [
        { required: true, message: "Please provide a name" },
        { min: 5, max: 50, message: "Name must be between 5 and 50 characters" }
    ],
    jersey: [
        { required: true, message: "Please provide a jersey number" },
        { type: "number", min: 0, max: 199, message: "Jersey number must be between 0 and 199",
          transform: (value: string) => parseInt(value) }
    ],
    position: [
        { required: false, message: "Please provide a position" },
        { min: 0, max: 16, message: "Position must be between 0 and 16 characters" }
    ],
    height: [
        { required: false, message: "Height, e.g. 6'2\"." },
        HEIGHT_VALIDATOR
    ],
    weight: [
        { required: false, message: "Weight, e.g. 150 (lbs)." },
        { type: "number", min: 0, message: "Weight must be at least 0.",
          transform: (value: string) => { parseInt(value) }}
    ],
    year: [
        { required: false, message: "Provide grade number or graduation year" },
        { type: "number", min: 0, max: 2050, message: "Year (grade number or year).",
          transform: (value: string) => parseInt(value) }
    ]
});

export const DEVICE_CONFIG_VALIDATOR = (deviceConfig: DeviceConfig) => new Schema({
    data_src: [
        { required: true, message: "Please provide a data source" },
        { type: "number", min: 0, max: 3, message: "Data source must be between 0 and 3" }
    ],
    data_fmt: [
        { required: true, message: "Please provide a data format" },
        { type: "number", min: 0, max: 1, message: "Data format must be between 0 and 1" }
    ],
    radio_channel: [
        { required: true, message: "Radio channel is required" },
        { type: "number", min: 0, max: 80, message: "Radio channel must be between 0 and 80",
          transform: (value: string) => parseInt(value) }
    ],
    radio_address: [
        { required: true, message: "Radio address is required" },
        { type: "number", min: 0, max: 65535, message: "Radio address must be between 0 and 65535",
          transform: (value: string) => parseInt(value) }
    ],
    radio_encrypt: [
        { type: "number", min: 0, max: 65535, message: "Radio encryption key must be between 0 and 65535",
            transform: (value: string) => parseInt(value) }
    ],
    ...(deviceConfig.data_src === ScoreDataSource.SRC_WEB && {
        web_addr: [
        { required: true, message: "Web Server address or host is required" },
        IP_OR_HOSTNAME_VALIDATOR
      ],
      web_port: [
        { required: true, message: "Web Port is required" },
        { type: "number", min: 0, max: 65535, message: "Web port must be between 0 and 65535",
            transform: (value: string) => parseInt(value) }
      ]
    })
});
