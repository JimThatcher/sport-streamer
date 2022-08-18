import Schema from "async-validator";
import { DeviceConfig, ScoreDataSource } from "./types";
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
