import React, { FC, useCallback, useState } from 'react';
import { Navigate, Routes, Route, useNavigate } from 'react-router-dom';

import { RequireAdmin, useLayoutTitle } from '../components';
import { SponsorData } from './types';
// import { AuthenticatedContext } from '../contexts/authentication';
import { SponsorContext } from './OverlayAssetsContext';
import SponsorListForm from './SponsorListForm';
import SponsorSetupForm from './SponsorSetupForm';
import SponsorAddForm from './SponsorAddForm';

const SponsorMain: FC = () => {
  useLayoutTitle("Sponsor Manager");

  // const authenticatedContext = useContext(AuthenticatedContext);
  const navigate = useNavigate();

  const [selectedSponsor, setSelectedSponsor] = useState<SponsorData>();

  const selectSponsor = useCallback((sponsor: SponsorData) => {
    setSelectedSponsor(sponsor);
    navigate('setup');
  }, [navigate]);

  const deselectSponsor = useCallback(() => {
    setSelectedSponsor(undefined);
  }, []);

  return (
    <SponsorContext.Provider
      value={{
        selectedSponsor: selectedSponsor,
        selectSponsor: selectSponsor,
        deselectSponsor: deselectSponsor
      }}
    >
      <Routes>
        <Route path="list" element={<SponsorListForm />} />
        <Route
          path="setup"
          element={
            <RequireAdmin>
              <SponsorSetupForm />
            </RequireAdmin>
          }
        />
        <Route
          path="add"
          element={
            <RequireAdmin>
              <SponsorAddForm />
            </RequireAdmin>
          }
        />
        <Route path="/*" element={<Navigate replace to="list" />} />
      </Routes>
    </SponsorContext.Provider>
  );

};

export default SponsorMain;
