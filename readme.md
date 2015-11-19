# slackmeme
This is a meme generator for Slack. 

## This is probably awful.
It implements a command web hook for accepting commands from Slack, parsing the input text and generating one of several meme images in response. These images are stored, and a URL returned in a JSON message back to Slack.

## Hey, it has tests. Well sort of.
The test library is mostly incomplete. This was hacked together in a couple of hours, and since then I've been using it as an excuse to play with things I don't normally get to fool with at work (Nancy, xUnit, etc.)

## It uses Azure. Which might be a good thing, or not.
I'm using Ninject for DI, and my actual store is abstracted behind an interface, so while what you see here is using Azure Blob Storage to store the file, you could easily supply your own IBlobStore implementation and store the generated images elsewhere.

## It doesn't do some important things.
Namely, it doesn't rate-limit meme requests. It also doesn't filter requests by Slack token, so you could get DOS'd pretty easily. It's not wrapped up as a nice Slack app, so you have to configure it manually.

*I mentioned that I did this in a couple of hours, right?*

[![Build Status](https://travis-ci.org/brporter/slackmeme.svg)](https://travis-ci.org/brporter/slackmeme)
