import { Signal, computed } from '@angular/core';

/**
 * Maps a list stored under a specific key in the store's state by a target key.
 * @param stateKey - The key of the list.
 * @param targetKey - The key used to identify the items.
 * @returns A computed function that returns the mapped list.
 */
export const mapByKey =
  <TargetKey>(targetKey: keyof TargetKey) =>
  <State>(state: Signal<State[]>) =>
    computed(() => state().map((item: any) => item[targetKey]));

/**
 * Maps a list stored under a specific key in the store's state by multiple target keys.
 * @param targetKeys - The keys used to identify the items.
 * @returns A computed function that returns the mapped list.
 */
export const mapByKeys =
  <TargetKey>(targetKeys: (keyof TargetKey)[]) =>
  <State>(state: Signal<State[]>) =>
    computed(() =>
      state().map((item: any) =>
        targetKeys.reduce((acc, key) => ({ ...acc, [key]: item[key] }), []),
      ),
    );

/**
 * Finds an item in a list stored under a specific key in the store's state by a target key value.
 * @param stateKey - The key of the list.
 * @param targetKey - The key used to identify the item.
 * @returns A computed function that returns the found item.
 */
export const findByKey =
  <TargetKey>(targetKey: keyof TargetKey) =>
  <State>(state: Signal<State>, value: string | number | boolean) =>
    computed(() =>
      (<any>state)().find((item: any) => item[targetKey] === value),
    );

/**
 * Finds an item in a list stored under a specific key in the store's state by a deep key value.
 * @param deepProperty - The property used to access the deep key.
 * @param deepKey - The deep key used to identify the item.
 * @returns A computed function that returns the found item.
 */
export const findByDeepKey =
  <DeepProp, DeepKey>(deepProperty: keyof DeepProp, deepKey: keyof DeepKey) =>
  <State>(state: Signal<State[]>, value: string | number | boolean) =>
    computed(() =>
      state().find((item: any) => item[deepProperty][deepKey] === value),
    );

/**
 * Filters a list stored under a specific key in the store's state by a target key value.
 * @param stateKey - The key of the list.
 * @param targetKey - The key used to identify the items.
 * @returns A computed function that returns the filtered list.
 */
export const filterByKey =
  <TargetKey>(targetKey: keyof TargetKey) =>
  <State>(state: Signal<State[]>, value: string | number | boolean) =>
    computed(() =>
      state().filter((item: any) =>
        item[targetKey]
          .toString()
          .toLowerCase()
          .includes(value.toString().toLowerCase()),
      ),
    );

/**
 * Filters a list stored under a specific key in the store's state by a deep key value.
 * @param deepProperty - The property used to access the deep key.
 * @param deepKey - The deep key used to identify the items.
 * @returns A computed function that returns the filtered list.
 */
export const filterByDeepKey =
  <DeepProp, DeepKey>(deepProperty: keyof DeepProp, deepKey: keyof DeepKey) =>
  <State>(state: Signal<State[]>, value: string | number | boolean) =>
    computed(() =>
      state().filter((item: any) =>
        item[deepProperty][deepKey]
          .toString()
          .toLowerCase()
          .includes(value.toString().toLowerCase()),
      ),
    );
