# 🎵 Jam Junction

> Your ultimate Discord music bot for orchestrating the perfect listening experience.

[![Top.gg](https://img.shields.io/badge/Top.gg-Jam%20Junction-red?logo=top.gg)](https://top.gg/bot/1181700334561796227)
[![Invite](https://img.shields.io/badge/Discord-Invite%20Bot-5865F2?logo=discord&logoColor=white)](https://discord.com/oauth2/authorize?client_id=1181700334561796227)
[![Ko-fi](https://img.shields.io/badge/Ko--fi-Donate-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/jamjunction)

---

![Bot Card](/Images/Bot-Card.png)

---

## Overview

Jam Junction is a feature-rich Discord music bot built with DSharpPlus and Lavalink4NET. It supports multiple streaming platforms, full queue management, audio filters, repeat modes, and an interactive player with button controls — all within your Discord server.

---

## Features

- **6 Supported Platforms** — Spotify, YouTube, YouTube Music, Deezer, SoundCloud, Apple Music
- **Full Queue Management** — Add, remove, skip, shuffle, and paginate up to 100 tracks
- **Previous Track Support** — Navigate back through your playback history
- **Seek & Position** — Jump to any point in a track using hours, minutes, or seconds
- **Audio Filters** — Nightcore, 8D, Vaporwave, Karaoke, Slow Motion, and Reset
- **Repeat Modes** — None, Repeat Track, Repeat Queue
- **Volume Control** — Adjustable from 0 to 100
- **Interactive Player** — 15 button controls directly on the player embed
- **AI Song Recommendations** — An offline local LLM suggests a similar track after playback, ready to add to the queue with one click
- **Synced Lyrics** — Fetch time-synced lyrics for the current track, sourced from LRCLIB
- **Personal Playlist** — Like any track to save it to your DMs for later
- **Help Menu** — Built-in dropdown help system with categorized sections

---

## Supported Platforms

| Platform | Tracks | Albums | Playlists |
|----------|:------:|:------:|:---------:|
| <img src="https://cdn.simpleicons.org/spotify" width="16" height="16"> Spotify | ✅ | ✅ | ✅ |
| <img src="https://cdn.simpleicons.org/youtube" width="16" height="16"> YouTube | ✅ | — | ✅ |
| <img src="https://cdn.simpleicons.org/youtubemusic" width="16" height="16"> YouTube Music | ✅ | — | ✅ |
| <img src="https://cdn.simpleicons.org/deezer" width="16" height="16"> Deezer | ✅ | ✅ | ✅ |
| <img src="https://cdn.simpleicons.org/soundcloud" width="16" height="16"> SoundCloud | ✅ | — | ✅ |
| <img src="https://cdn.simpleicons.org/applemusic" width="16" height="16"> Apple Music | ✅ | ✅ | ✅ |

---

## Commands

### 🎵 Music Commands

| Command | Description |
|---------|-------------|
| `/play` | Queue a track by keyword or URL. |
| `/pause` | Pauses the current track. |
| `/resume` | Resumes the current track. |
| `/stop` | Stops the playback. |
| `/restart` | Restarts the current track. |
| `/previous-track` | Returns to the previously played track. |
| `/skip` | Skips to the next track in the queue. |
| `/skip-to` | Skips to the desired track in the queue. |
| `/remove` | Removes a track from the queue. |
| `/shuffle` | Shuffles the queue. |
| `/view-queue` | Displays what is currently in the queue. |
| `/current-track` | Shows details about the current track playing. |
| `/lyrics` | Shows lyrics for the current track. |
| `/seek` | Sets the position of the track. |
| `/position` | Gets the current track position. |
| `/volume` | Adjust the volume 0-100. |
| `/filters` | Change the audio filter for the player. |
| `/repeating-mode` | Change the repeating mode. |
| `/leave` | Disconnects the player. |

### 🛠️ Other Commands

| Command | Description |
|---------|-------------|
| `/help` | Gives information about the bot & available commands. |
| `/personal-playlist` | Opens your personal playlist in the bot's DMs. |
| `/ping` | Will pong back to the server. |
| `/caption` | Give any image a caption. |

---

## Player Controls

The interactive player embed includes 15 buttons across 3 rows.

| Row | Buttons |
|-----|---------|
| **1** | ⏸ Pause · ⏮ Previous Track · ▶ Resume · ⏭ Skip · ⏹ Stop |
| **2** | ☰ View Queue · 🔉 Volume Down · 🔊 Volume Up · ↻ Restart · ⇄ Repeat |
| **3** | ⇌ Shuffle · ❤️ Like · 🛟 Help · 📋 Playlist · 🔍 Seek |

> ⏮ is disabled when there is no playback history. 🔉 / 🔊 are disabled at min / max volume. ☰ and ⇌ are disabled when the queue is empty.

---

## Tech Stack

- [DSharpPlus 5.0.0](https://github.com/DSharpPlus/DSharpPlus) — Discord API wrapper
- [Lavalink4NET 4.2.0](https://github.com/angelobreuer/Lavalink4NET) — Audio streaming
- [Ollama](https://ollama.com) — Local LLM powering AI song recommendations
- [LRCLIB](https://lrclib.net) — Synced lyrics provider
- .NET 9
- [Docker](https://www.docker.com/) — Containerized deployment

---

## Links

| | |
|---|---|
| 🤖 **Invite** | [Add Jam Junction to your server](https://discord.com/oauth2/authorize?client_id=1181700334561796227) |
| 🎩 **Top.gg** | [Rate & review Jam Junction](https://top.gg/bot/1181700334561796227) |
| ☕ **Ko-fi** | [Support development](https://ko-fi.com/jamjunction) |
| 🌊 **Hosting** | [Digital Ocean](https://www.digitalocean.com) — great and affordable bot hosting |
