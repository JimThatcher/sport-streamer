import { useCallback, useEffect, useState } from "react";
import { useSnackbar } from 'notistack';
import { AxiosPromise } from "axios";

import { extractErrorMessage } from ".";

export interface RestRequestOptionsParam<D> {
    param: string;
    read: (value: string) => AxiosPromise<D>;
    update?: (value: D, id: string) => AxiosPromise<D>;
}

export const useRestParam = <D>({ param, read, update }: RestRequestOptionsParam<D>) => {
  const { enqueueSnackbar } = useSnackbar();

  const [saving, setSaving] = useState<boolean>(false);
  const [data, setData] = useState<D>();
  const [errorMessage, setErrorMessage] = useState<string>();

  const loadData = useCallback(async () => {
    setData(undefined);
    setErrorMessage(undefined);
    try {
      setData((await read(param)).data);
    } catch (error: any) {
      const message = extractErrorMessage(error, 'Problem loading data');
      enqueueSnackbar(message, { variant: 'error' });
      setErrorMessage(message);
    }
  }, [read, enqueueSnackbar, param]);

  const save = useCallback(async (toSave: D) => {
    if (!update) {
      return;
    }
    setSaving(true);
    setErrorMessage(undefined);
    try {
      setData((await update(toSave, param)).data);
      enqueueSnackbar("Update successful", { variant: 'success' });
    } catch (error: any) {
      const message = extractErrorMessage(error, 'Problem saving data');
      enqueueSnackbar(message, { variant: 'error' });
      setErrorMessage(message);
    } finally {
      setSaving(false);
    }
  }, [update, enqueueSnackbar, param]);

  const saveData = () => data && save(data);

  useEffect(() => {
    loadData();
  }, [loadData]);

  return { loadData, saveData, saving, setData, data, errorMessage } as const;
};
