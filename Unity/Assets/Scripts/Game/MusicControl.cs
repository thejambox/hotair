using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;   

public class MusicControl : MonoBehaviour
{
    private List<AudioSource> sources;

    private Dictionary<string, Dictionary<string, List<string>>> songMap;
    private List<string> files;
    private Dictionary<string, AudioClip> clips;
    private List<AudioClip> playlist;

    private string lastEnd;

    private double nextAudioStart = -1;

    private string pathMusic
    {
        get { return Path.GetFullPath(Application.dataPath + "/../Music/"); }
    }

    private void Awake()
    {
        lastEnd = "A"; // always start at A

        CreateAudioSources(4);
        SetupSongMap();
        StartCoroutine(LoadSongs());
    }

    private void Start()
    {
        StartCoroutine(PlayMusic());
    }

    private void CreateAudioSources(int count)
    {
        sources = new List<AudioSource>();

        for (int i = 0; i < count; ++i)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sources.Add(source);
        }
    }

    private void SetupSongMap()
    {
        songMap = new Dictionary<string, Dictionary<string, List<string>>>();
        files = new List<string>(Directory.GetFiles(pathMusic, "*.wav"));

        for (int i = 0; i < files.Count; ++i)
        {
            string mp3 = Path.GetFileNameWithoutExtension(files[i]);
            int idxUnder = mp3.LastIndexOf("_");
            string start = mp3.Substring(idxUnder + 1, 1);
            string end = mp3.Substring(idxUnder + 2, 1);

            if (!songMap.ContainsKey(start))
                songMap.Add(start, new Dictionary<string, List<string>>());

            if (!songMap[start].ContainsKey(end))
                songMap[start].Add(end, new List<string>());

            songMap[start][end].Add(mp3);
        }
    }

    private IEnumerator LoadSongs()
    {
        clips = new Dictionary<string, AudioClip>();

        for (int i = 0; i < files.Count; ++i)
        {
            string shortName = Path.GetFileNameWithoutExtension(files[i]);

            if (clips.ContainsKey(shortName))
            {
                Debug.LogError("Somehow, this file already exists in a clip?");
            }
            else
            {
                WWW loadedMusic = new WWW("file://" + files[i]);

                while (!loadedMusic.isDone)
                    yield return null;
             
                AudioClip clip = loadedMusic.GetAudioClip(false, false);
                clip.name = shortName;

                clips.Add(shortName, clip);

                loadedMusic = null;
            }
        }

        AddToPlaylist(10);
    }

    private void AddToPlaylist(int fillCount)
    {
        if (playlist == null)
            playlist = new List<AudioClip>();

        for (int i = 0; i < fillCount; ++i)
        {
            string start = lastEnd;

            if (songMap.ContainsKey(start))
            {
                List<string> ends = new List<string>(songMap[start].Keys);

                string end = ends.GetRandom();

                string song = songMap[start][end].GetRandom();

                playlist.Add(clips[song]);

                Debug.Log("pl: " + song);

                lastEnd = end;
            }
            else
            {
                Debug.LogError("Terminated early because we hit a dead end: " + start);
                break;
            }
        }
    }

    private IEnumerator PlayMusic()
    {
        nextAudioStart = AudioSettings.dspTime + 1f;
        int sourcesUsed = 0;

        while (playlist == null)
            yield return null;

        while (playlist.Count > 0)
        {
            if (sourcesUsed < sources.Count)
            {
                for (int i = 0; i < sources.Count; ++i)
                {
                    if (sources[i].isPlaying)
                        continue;

                    if (playlist.Count == 0)
                        break;

                    sources[i].clip = playlist.Pop(0);
                    sources[i].PlayScheduled(nextAudioStart);

                    nextAudioStart += sources[i].clip.length;
                    ++sourcesUsed;

                    //Debug.LogWarning(sources[i].clip.name);
                }
            }

            for (int i = 0; i < sources.Count; ++i)
            {
                if (!sources[i].isPlaying)
                    --sourcesUsed;
            }

            // could always add more to the playlist here if we need more.
            yield return null;
        }

        //Debug.Log("Done");
    }
}
