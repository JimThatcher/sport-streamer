import Schema from "async-validator";

import { WiFiSettings } from "../types";

import { HOSTNAME_VALIDATOR, IP_ADDRESS_VALIDATOR } from "./shared";

export const createWiFiSettingsValidator = (wifiSettings: WiFiSettings) => new Schema({
  ssid: [
    { required: true, message: "SSID is required" },
    { type: "string", max: 32, message: "SSID must be 32 characters or less" }
  ],
  password: { type: "string", max: 64, message: "Password must be 64 characters or less" },
  hostname: [
    { required: true, message: "Hostname is required" },
    HOSTNAME_VALIDATOR
  ],
  ...(wifiSettings.static_ip_config && {
    local_ip: [
      { required: true, message: "Local IP is required" },
      IP_ADDRESS_VALIDATOR
    ],
    gateway_ip: [
      { required: true, message: "Gateway IP is required" },
      IP_ADDRESS_VALIDATOR
    ],
    subnet_mask: [
      { required: true, message: "Subnet mask is required" },
      IP_ADDRESS_VALIDATOR
    ],
    dns_ip_1: IP_ADDRESS_VALIDATOR,
    dns_ip_2: IP_ADDRESS_VALIDATOR,
  })
});
