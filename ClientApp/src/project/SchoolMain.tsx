import React, { FC, useCallback, useState } from 'react';
import { Navigate, Routes, Route, useNavigate } from 'react-router-dom';

import { RequireAdmin, useLayoutTitle } from '../components';
import { SchoolData } from './types';
// import { AuthenticatedContext } from '../contexts/authentication';
import { SchoolContext } from './SchoolContext';
import SchoolListForm from './SchoolListForm';
import SchoolSetupForm from './SchoolSetupForm';
import SchoolAddForm from './SchoolAddForm';

const SchoolMain: FC = () => {
  useLayoutTitle("School Manager");

  // const authenticatedContext = useContext(AuthenticatedContext);
  const navigate = useNavigate();

  const [selectedSchool, setSelectedSchool] = useState<SchoolData>();

  const selectSchool = useCallback((school: SchoolData) => {
    setSelectedSchool(school);
    navigate('setup');
  }, [navigate]);

  const deselectSchool = useCallback(() => {
    setSelectedSchool(undefined);
  }, []);

  return (
    <SchoolContext.Provider
      value={{
        selectedSchool,
        selectSchool,
        deselectSchool
      }}
    >
      <Routes>
        <Route path="list" element={<SchoolListForm />} />
        <Route
          path="setup"
          element={
            <RequireAdmin>
              <SchoolSetupForm />
            </RequireAdmin>
          }
        />
        <Route
          path="add"
          element={
            <RequireAdmin>
              <SchoolAddForm />
            </RequireAdmin>
          }
        />
        <Route path="/*" element={<Navigate replace to="list" />} />
      </Routes>
    </SchoolContext.Provider>
  );

};

export default SchoolMain;
