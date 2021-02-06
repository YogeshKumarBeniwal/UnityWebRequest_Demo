﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YogeshBen.Stackeer;

namespace UnityWebRequestDemo
{
    public class ButtonViewManager : MonoBehaviour
    {
        [SerializeField]
        private Button actionButton;
        [SerializeField]
        private TextMeshProUGUI buttonText;
        [SerializeField]
        private TextMeshProUGUI progressText;

        private bool isDataCached = false;

        public delegate void DownloadDoneAction();
        public static event DownloadDoneAction OnDownloadDone;

        public void SetView()
        {
            if (Stackeer.IsFileAlreadyExists(Global.cloudDataUri))
            {
                isDataCached = true;
                OnDataAlreadyCached();
            }
            else
            {
                isDataCached = false;
                OnDownloadRequired();
            }
        }

        private void OnDataAlreadyCached()
        {
            buttonText.text = "Show";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {

                buttonText.text = "Showing...";
                actionButton.interactable = false;
                Stackeer.Get().Load(Global.cloudDataUri).SetWebRequestType(WEB_REQUEST_TYPE.HTTP_GET)
                .WithJsonLoadedAction(OnJsonLoaded).WithErrorAction(OnDownloadFailed).WithLoadedAction(OnDataReady)
                .SetEnableLog(true).StartStackeer();
            });
        }

        private void OnDownloadRequired()
        {
            buttonText.text = "Fetch";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                buttonText.text = "Fetching...";
                actionButton.interactable = false;
                Stackeer.Get().Load(Global.cloudDataUri).SetWebRequestType(WEB_REQUEST_TYPE.HTTP_GET)
                .WithJsonLoadedAction(OnJsonLoaded).WithErrorAction(OnDownloadFailed).WithLoadedAction(OnDataDownloaded)
                .SetEnableLog(true).StartStackeer();
            });
        }

        private void OnJsonLoaded(string data)
        {
            Global.loadedServerData = JsonConverter.DeserializeObject<Root>(data);
        }

        private void OnDownloadFailed(string error)
        {
            actionButton.interactable = true;
            if (isDataCached)
                buttonText.text = "Show";
            else
                buttonText.text = "Fatch";

            progressText.color = Color.red;
            progressText.text = "Retry!";
        }

        private void OnDataDownloaded()
        {
            actionButton.interactable = true;
            buttonText.text = "Show";
            progressText.color = Color.green;
            progressText.text = "Done!";

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                OnDataReady();
            });
        }

        private void OnDataReady()
        {
            OnDownloadDone();
        }
    }
}