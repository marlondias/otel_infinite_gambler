# The Infinite Gambler Simulator

A simplified simulation of games of luck, random outcomes and gambling addiction.

## The simulation

There are many gambling companies (`Casino` class), each providing diverse types of games of chance (`Game` class).

Games have varying levels of difficulty (odds) and participation fees.
The prize is proportional to the cost and risk involved.
The outcome of these games is always based on random chance, not influenced by skill or environment conditions.

There are many players (`Player` class) who dedicate all of their time and money to these games.
A player's dream is to get rich and buy his own Casino.

Each player starts with some amount of money, enough to cover some rounds of betting.
Whenever a player wins, the value of the prize will be instantly transfered to his account.

The simulation will run for indeterminate amount of time, stopping only when either:

- The players loose all of their money.
- Some player has enough money to buy one of the casinos.

## The real goal

This is a toy project for improving my skills with systems observability and its tools.

The main application (.NET console) is only a backend with heavy instrumentation for generating OpenTelemetry signals.
These signals are sent to different dedicated services, then finally consumed by Grafana for making a dashboard.

In other words, the Grafana dashboard is the "frontend" for this project.

> Note: Telemetry signals are not supposed to be used as transport for core/business data. That was just part of the challenge. Don't do that in a real project.

## Technologies

- **.NET Core** for the backend of the simulation.
- **OpenTelemetry** for producing, collecting and routing the telemetry signals.
- **Loki** for processing application logs.
- **Prometheus** for processing application metrics.
- **Grafana** for the visuals of the simulation.

## Installation

1) You must have [Docker](https://www.docker.com/) properly installed on your device.

2) Open a terminal at the root of this project.

3) Run `cp .env.example .env`.

4) Edit the `.env` file, replacing the placeholder values.

5) Run `docker compose up --build -d` and wait for the process to finish.

6) To run a simulation use `docker exec -it ig_app dotnet run`.

7) To see the telemetry for the simulation, access the [Grafana UI](http://localhost:3000) on your browser.

> Note: You need to run the simulation at least once to see any data in Grafana.
