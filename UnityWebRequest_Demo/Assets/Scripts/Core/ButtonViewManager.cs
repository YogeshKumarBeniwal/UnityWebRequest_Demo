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
        private Button binButton;
        [SerializeField]
        private TextMeshProUGUI buttonText;
        [SerializeField]
        private TextMeshProUGUI progressText;

        private bool isDataCached = false;

        public delegate void CloudDataReadyAction();
        public static event CloudDataReadyAction OnCloudDataReady;

        private void Awake()
        {
            binButton.onClick.AddListener(() =>
            {
                ClearAllCachedData();
                SetView();
            });
        }

        public void SetView()
        {
            progressText.text = "Let's GO";

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

        private void ClearAllCachedData()
        {
            Stackeer.ClearAllCachedFiles();
        }

        private void OnDataAlreadyCached()
        {
            ToggleBinButtonVisiblity(true);
            buttonText.text = "Show";
            progressText.color = Color.green;
            progressText.text = "Done!";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                progressText.text = "0%";
                progressText.color = Color.black;
                buttonText.text = "Showing...";
                actionButton.interactable = false;
                Stackeer.Get().Load(Global.cloudDataUri).SetWebRequestType(WEB_REQUEST_TYPE.HTTP_GET)
                .WithGetResponseLoadedAction(OnJsonLoaded).WithErrorAction(OnDownloadFailed).WithLoadedAction(OnDataReady)
                .WithDownloadProgressChangedAction(UpdateProgress).StartStackeer();
            });
        }

        private void OnDownloadRequired()
        {
            ToggleBinButtonVisiblity(false);
            buttonText.text = "Fetch";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                progressText.text = "0%";
                progressText.color = Color.black;
                buttonText.text = "Fetching...";
                actionButton.interactable = false;
                Stackeer.Get().Load(Global.cloudDataUri).SetWebRequestType(WEB_REQUEST_TYPE.HTTP_GET)
                .WithGetResponseLoadedAction(OnJsonLoaded).WithErrorAction(OnDownloadFailed).WithLoadedAction(OnDataDownloaded)
                .WithDownloadProgressChangedAction(UpdateProgress).StartStackeer();
            });
        }

        private void UpdateProgress(int progress)
        {
            progressText.text = progress + "%";
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
            ToggleBinButtonVisiblity(true);

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                OnDataReady();
            });
        }

        private void OnDataReady()
        {
            OnCloudDataReady();
        }

        /// <summary>
        /// Toggle bin button Visiblity
        /// if 'true' button will be visible else invisible
        /// </summary>
        /// <param name="showButton"></param>
        private void ToggleBinButtonVisiblity(bool showButton)
        {
            binButton.gameObject.SetActive(showButton);
        }
    }
}
