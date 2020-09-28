﻿using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;

namespace SavingsProjection.SPA.Pages
{
    public partial class RecurrentItemEdit : ComponentBase
    {
        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public RecurrentMoneyItem recurrentItemToEdit { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.recurrentItemToEdit.StartDate = DateTime.Now;
            }
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (isNew)
                {
                    await savingProjectionAPI.InsertRecurrentMoneyItem(recurrentItemToEdit);
                }
                else
                {
                    await savingProjectionAPI.EditRecurrentMoneyItem(recurrentItemToEdit.ID, recurrentItemToEdit);
                }
                this.dialogService.Close(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
