import React, { FC, RefObject } from 'react';
import { SnackbarProvider } from 'notistack';

import { IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { FeaturesLoader } from './contexts/features';

import CustomTheme from './CustomTheme';
import AppRouting from './AppRouting';

// imports for date/time localization
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

const App: FC = () => {
  const notistackRef: RefObject<any> = React.createRef();

  const onClickDismiss = (key: string | number | undefined) => () => {
    notistackRef.current.closeSnackbar(key);
  };
  // console.log("URL: %s", window.location.href);

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <CustomTheme>
        <SnackbarProvider
          maxSnack={3}
          anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
          ref={notistackRef}
          action={(key) => (
            <IconButton onClick={onClickDismiss(key)} size="small">
              <CloseIcon />
            </IconButton>
          )}
        >
          <FeaturesLoader>
            <AppRouting />
          </FeaturesLoader>
        </SnackbarProvider>
      </CustomTheme>
    </LocalizationProvider>
  );
};

export default App;
