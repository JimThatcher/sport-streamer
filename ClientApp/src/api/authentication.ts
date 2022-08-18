import { AxiosPromise } from "axios";
import * as H from 'history';
import jwtDecode from 'jwt-decode';
import { Path } from "react-router-dom";

import { Features, Me, SignInRequest, SignInResponse } from "../types";

import { ACCESS_TOKEN, AXIOS } from "./endpoints";
import { PROJECT_PATH } from './env';

export const SIGN_IN_PATHNAME = 'loginPathname';
export const SIGN_IN_SEARCH = 'loginSearch';

export const getDefaultRoute = (features: Features) => features.project ? "/schlmgr" : "/wifi";

export function verifyAuthorization(): AxiosPromise<void> {
  return AXIOS.get('/verifyAuthorization');
}

export function signIn(request: SignInRequest): AxiosPromise<SignInResponse> {
  return AXIOS.post('/signIn', request);
}

/**
 * Fallback to sessionStorage if localStorage is absent. WebView may not have local storage enabled.
 */
export function getStorage() {
  return localStorage || sessionStorage;
}

export function storeLoginRedirect(location?: H.Location) {
  console.log("Auth: storing loc: %s", location?.pathname);
  if (location) {
    getStorage().setItem(SIGN_IN_PATHNAME, location.pathname);
    getStorage().setItem(SIGN_IN_SEARCH, location.search);
    console.log("Auth: Stored path: %s", localStorage.getItem(SIGN_IN_PATHNAME));
  }
}

export function clearLoginRedirect() {
  getStorage().removeItem(SIGN_IN_PATHNAME);
  getStorage().removeItem(SIGN_IN_SEARCH);
}

export function fetchLoginRedirect(features: Features): Partial<Path> {
  const signInPathname = getStorage().getItem(SIGN_IN_PATHNAME);
  const signInSearch = getStorage().getItem(SIGN_IN_SEARCH);
  // clearLoginRedirect();
  return {
    pathname: signInPathname || getDefaultRoute(features),
    search: (signInPathname && signInSearch) || undefined
  };
}

export const clearAccessToken = () => localStorage.removeItem(ACCESS_TOKEN);
export const decodeMeJWT = (accessToken: string): Me => jwtDecode(accessToken) as Me;

export function addAccessTokenParameter(url: string) {
  const accessToken = getStorage().getItem(ACCESS_TOKEN);
  if (!accessToken) {
    return url;
  }
  const parsedUrl = new URL(url);
  parsedUrl.searchParams.set(ACCESS_TOKEN, accessToken);
  return parsedUrl.toString();
}
