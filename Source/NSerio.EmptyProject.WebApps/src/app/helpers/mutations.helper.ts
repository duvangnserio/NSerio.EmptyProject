import { StateSignal, patchState } from '@ngrx/signals';

/**
 * Sets the value of a specific key in the store's state.
 * @param stateKey - The key to set.
 * @returns A function that accepts the store and the value to set.
 */
export const set =
  <StateKey>(stateKey: keyof StateKey) =>
  (store: StateSignal<object>, value: unknown) => {
    patchState(store, { [stateKey]: value });
  };

/**
 * Toggles the value of a specific key in the store's state.
 * @param stateKey - The key to toggle.
 * @returns A function that accepts the store.
 */
export const toggle =
  <StateKey>(stateKey: keyof StateKey) =>
  (store: StateSignal<object>) => {
    patchState(store, (state: any) => ({ [stateKey]: !state[stateKey] }));
  };

/**
 * Replaces a record in a list stored under a specific key in the store's state.
 * @param stateKey - The key of the list.
 * @param targetKey - The key used to identify the record (default: 'artifactId').
 * @returns A function that accepts the store, the value to replace, and the idKey.
 */
export const replaceRecordInList =
  <StateKey, TargetKey>(stateKey: keyof StateKey, targetKey: keyof TargetKey) =>
  (state: StateSignal<object>, value: any) => {
    patchState(state, (state: any) => ({
      [stateKey]: state[stateKey].map((item: any) =>
        item[targetKey] === value[targetKey] ? value : item,
      ),
    }));
  };

/**
 * Removes a record from a list stored under a specific key in the store's state.
 * @param stateKey - The key of the list.
 * @param targetKey - The key used to identify the record (default: 'artifactId').
 * @returns A function that accepts the store and the value to remove.
 */
export const removeRecordFromList =
  <StateKey, TargetKey>(stateKey: keyof StateKey, targetKey: keyof TargetKey) =>
  (store: StateSignal<object>, value: any) => {
    patchState(store, (state: any) => ({
      [stateKey]: state[stateKey].filter((item: any) => {
        if (typeof value === 'object') {
          return item[targetKey] !== value[targetKey];
        } else {
          return item[targetKey] !== value;
        }
      }),
    }));
  };

/**
 * Adds a record to a list stored under a specific key in the store's state.
 * @param stateKey - The key of the list.
 * @returns A function that accepts the store and the value to add.
 */
export const addRecordToList =
  <StateKey>(stateKey: keyof StateKey) =>
  (store: StateSignal<object>, value: unknown) => {
    patchState(store, (state: any) => ({
      [stateKey]: [...state[stateKey], value],
    }));
  };

/**
 * Pushes an array of values to a list stored under a specific key in the store's state.
 * @param stateKey - The key of the list.
 * @returns A function that accepts the store and the array of values to push.
 */
export const pushArray =
  <StateKey>(stateKey: keyof StateKey) =>
  (store: StateSignal<object>, value: any) => {
    patchState(store, (state: any) => ({
      [stateKey]: [...state[stateKey], ...value],
    }));
  };
