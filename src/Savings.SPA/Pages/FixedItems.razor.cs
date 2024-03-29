﻿using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savings.SPA.Pages
{
    public partial class FixedItems : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        public Configuration CurrentConfiguration { get; set; }

        private FixedMoneyItem[] fixedMoneyItems;

        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        public long? FilterCategory { get; set; }

        public MoneyCategory[] Categories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateFrom = DateTime.Now.Date.AddMonths(-2);
            FilterDateTo = DateTime.Now.Date.AddDays(15);
            Categories = await savingsAPI.GetMoneyCategories();
            CurrentConfiguration = (await savingsAPI.GetConfigurations()).FirstOrDefault();
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }

        async Task FilterCategoryChanged(ChangeEventArgs e)
        {
            var selectedString = e.Value.ToString();
            FilterCategory = string.IsNullOrWhiteSpace(selectedString) ? null : long.Parse(selectedString);

            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            fixedMoneyItems = await savingsAPI.GetFixedMoneyItems(FilterDateFrom, FilterDateTo, false, FilterCategory);
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new Savings.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        async Task Edit(FixedMoneyItem item)
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", item }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }
    }
}
