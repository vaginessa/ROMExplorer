﻿// 
//  ROMExplorer
// 
//  Copyright 2018 Martin Gerczuk
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software 
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ROMExplorer.Sin;
using SharpCompress.Archives.Zip;

namespace ROMExplorer.Ftf
{
    internal class FtfFileInfo : FileInfoBase
    {
        private readonly List<ArchiveEntryViewModelBase> archiveEntries = new List<ArchiveEntryViewModelBase>();
        private readonly FileStream fileStream;
        private readonly ZipArchive zip;

        public FtfFileInfo(string filename)
        {
            fileStream = new FileStream(filename, FileMode.Open);
            zip = ZipArchive.Open(fileStream);

            var root0 = new DirectoryArchiveEntryViewModel();
            root0.InitDirectories(zip.Entries.Select(e => e.Key));
            root0.AddEntries(zip.Entries.Select(e => new FtfEntryViewModel(this, e)));
            archiveEntries.AddRange(root0.Children);
        }

        public void OpenSinStream(Stream stream)
        {
            Root = SinFileInfo.OpenSinStream(stream);
        }

        public class Factory : IFileInfoFactory
        {
            #region Implementation of IFileInfoFactory

            public string Filter { get; } = "Sony FTF Files (*.ftf)|*.ftf";

            public FileInfoBase Create(string filename)
            {
                return new FtfFileInfo(filename);
            }

            #endregion
        }

        #region Overrides of FileInfoBase

        public override void Dispose()
        {
            foreach (var entry in archiveEntries)
                entry.Dispose();

            archiveEntries.Clear();
            zip.Dispose();
            fileStream.Dispose();
        }

        public override IEnumerable<ArchiveEntryViewModelBase> ArchiveEntries => archiveEntries;

        #endregion
    }
}