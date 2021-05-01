# pack-man

A simple package manager that catalogs and installs packages organized into versioned artefacts. Each artefact can have a list of dependencies.

## Add

Add a new artefact to the store. The command takes either a zip file or a folder as argument.

## Extract

Extract a single artefact to a chosen destination folder. The artefact's dependencies are not expanded.

## Install

Install an artefact and its dependency to a chosen destination folder.

The artefact's dependency is extracted recursively to the same destination folder, in the order determined by the resolve command.

## Remove

Remove a single artefact from the store. Any artefacts that dependent on this artefact will be broken.

## Resolve

List the dependency chain of an artefact.

## Show

Browse packages and artefacts in the store.
