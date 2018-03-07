﻿// Copyright 1998-2017 Epic Games, Inc. All Rights Reserved.

using System;
using Tools.DotNETCommon;

public class CopyUsingDistillFileSet : BuildCommand
		bool UseExistingManifest = ParseParam("UseExistingManifest");

		List<string> SourceFiles;
		if (UseExistingManifest && FileExists_NoExceptions(ManifestFile))
		{
			// Read Source Files from Manifest
			List<string> Lines = new List<string>(ReadAllLines(ManifestFile));
			if (Lines.Count < 1)
			{
				throw new AutomationException("Manifest file {0} does not list any files.", ManifestFile);
			}
			SourceFiles = new List<string>();
			foreach (var ThisFile in Lines)
			{
				var TestFile = CombinePaths(ThisFile);
				if (!FileExists_NoExceptions(TestFile))
				{
					throw new AutomationException("GenerateDistillFileSets produced {0}, but {1} doesn't exist.", ThisFile, TestFile);
				}
				// we correct the case here
				var TestFileInfo = new FileInfo(TestFile);
				var FinalFile = CombinePaths(TestFileInfo.FullName);
				if (!FileExists_NoExceptions(FinalFile))
				{
					throw new AutomationException("GenerateDistillFileSets produced {0}, but {1} doesn't exist.", ThisFile, FinalFile);
				}
				SourceFiles.Add(FinalFile);
			}
		}
		else
		{
			// Run commandlet to get files required for maps
			FileReference Project = new FileReference(ProjectPath);
			string[] SplitMaps = null;
			if (!String.IsNullOrEmpty(Maps))
			{
				SplitMaps = Maps.Split(new char[] { '+', ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
			}
			SourceFiles = GenerateDistillFileSetsCommandlet(Project, ManifestFile, UE4Exe, SplitMaps, Parameters);
		}
		// Convert Source file paths to output paths and copy