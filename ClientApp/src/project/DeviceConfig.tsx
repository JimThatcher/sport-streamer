import { FC, useState } from 'react';
import { ValidateFieldsError } from 'async-validator';

import { Button, Checkbox, MenuItem } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';

import { SectionContent, FormLoader, BlockFormControlLabel, ButtonRow, ValidatedTextField } from '../components';
import { numberValue, updateValue, useRest } from '../utils';

import * as DeviceApi from './api';
import { DeviceConfig, ScoreDataSource, ScoreDataFormat } from './types';
import { validate } from "../validators";
import { DEVICE_CONFIG_VALIDATOR } from "./validators";

export const isMQTTEnabled = ({ mqtt_enabled }: DeviceConfig) => {
  return mqtt_enabled === true;
};

const DeviceConfigForm: FC = () => {
  const [fieldErrors, setFieldErrors] = useState<ValidateFieldsError>();
  const {
    loadData, saveData, saving, setData, data, errorMessage
  } = useRest<DeviceConfig>({ read: DeviceApi.readDeviceConfig, update: DeviceApi.updateDeviceConfig });

  const updateDataSource = (event: React.ChangeEvent<HTMLInputElement>) => {
    console.log(event.target.value);
    if (data && event.target.value == '0') {
      console.log('set data_src to SRC_RS232');
      data.data_fmt = ScoreDataFormat.FORMAT_RTD;
    }
    updateFormValue(event);
  };
  const updateFormValue = updateValue(setData);

  const content = () => {
    if (!data) {
      return (<FormLoader onRetry={loadData} errorMessage={errorMessage} />);
    }
    else {
      console.debug('DeviceConfigForm data:', data);
    }

    const validateAndSubmit = async () => {
      try {
        setFieldErrors(undefined);
        console.log('validateAndSubmit:', data);
        await validate(DEVICE_CONFIG_VALIDATOR(data), data);
        data.radio_address = +data.radio_address;
        data.radio_channel = +data.radio_channel;
        data.radio_encrypt = +data.radio_encrypt;
        data.web_port = +data.web_port;
        console.log('Saving:', data);
        saveData();
      } catch (errors: any) {
        console.log('catch errors');
        setFieldErrors(errors);
      }
    };

    return (
      <>
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="data_src"
          label="Score data source"
          value={(data.data_src !== undefined) ? data.data_src : ScoreDataSource.SRC_RS232}
          fullWidth
          select
          variant="outlined"
          onChange={updateDataSource}
          margin="normal"
        >
          <MenuItem value={ScoreDataSource.SRC_RS232}>RS232</MenuItem>
          <MenuItem value={ScoreDataSource.SRS_RADIO}>LoRa Radio</MenuItem>
          <MenuItem value={ScoreDataSource.SRC_WEB}>Web Socket</MenuItem>
          <MenuItem value={ScoreDataSource.SRC_MQTT}>MQTT (Not implemented)</MenuItem>
        </ValidatedTextField>
        {
          (data.data_src === ScoreDataSource.SRC_WEB) &&
          <>
            <ValidatedTextField
              sx={{ left: 40, width: 'calc(100% - 40px)' }}
              fieldErrors={fieldErrors}
              name="web_addr"
              label="Web Socket Server"
              fullWidth
              variant="outlined"
              value={data.web_addr}
              onChange={updateFormValue}
              margin="normal"
            />
            <ValidatedTextField
              sx={{ left: 40, width: 'calc(100% - 40px)' }}
              fieldErrors={fieldErrors}
              name="web_port"
              label="Web Socket Port"
              fullWidth
              variant="outlined"
              value={numberValue(data.web_port)}
              onChange={updateFormValue}
              margin="normal"
            />
          </>
        }
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="data_fmt"
          label="Score data format"
          value={(data.data_src === ScoreDataSource.SRC_RS232) ? ScoreDataFormat.FORMAT_RTD :
            (data.data_fmt !== undefined) ? data.data_fmt : ScoreDataFormat.FORMAT_RTD}
          fullWidth
          select
          variant="outlined"
          onChange={updateFormValue}
          margin="normal"
          disabled={data.data_src === ScoreDataSource.SRC_RS232}
        >
          <MenuItem value={ScoreDataFormat.FORMAT_RTD}>Daktronics RTD</MenuItem>
          <MenuItem value={ScoreDataFormat.FORMAT_JSON}>JSON</MenuItem>
        </ValidatedTextField>
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="radio_channel"
          label="Radio channel (0-80)"
          fullWidth
          variant="outlined"
          value={numberValue(data.radio_channel)}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="radio_address"
          label="Radio address (0-65535)"
          fullWidth
          variant="outlined"
          value={numberValue(data.radio_address)}
          onChange={updateFormValue}
          margin="normal"
        />
        <ValidatedTextField
          fieldErrors={fieldErrors}
          name="radio_encrypt"
          label="Radio encryption (0-65535)"
          fullWidth
          variant="outlined"
          value={numberValue(data.radio_encrypt)}
          onChange={updateFormValue}
          margin="normal"
        />
        <BlockFormControlLabel
          control={
            <Checkbox
              name="output_to_radio"
              disabled={saving}
              checked={data.output_to_radio}
              onChange={updateFormValue}
              color="primary"
            />
          }
          label="Send RS232 output to radio"
        />
        <BlockFormControlLabel
          control={
            <Checkbox
              name="mqtt_enabled"
              disabled={saving}
              checked={data.mqtt_enabled}
              onChange={updateFormValue}
              color="primary"
            />
          }
          label="Enable score updates to MQTT"
        />
        <ButtonRow mt={1}>
          <Button startIcon={<SaveIcon />} disabled={saving} variant="contained" color="primary" type="submit" onClick={validateAndSubmit}>
            Save
          </Button>
        </ButtonRow>
      </>
    );
  };

  return (
    <SectionContent title='Device configuration' titleGutter>
      {content()}
    </SectionContent>
  );
};

export default DeviceConfigForm;
