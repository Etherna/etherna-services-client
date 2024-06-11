// Copyright 2020-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using Etherna.Sdk.Internal.Clients;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AspNetCoreInternalClientSample.Pages;

public class IndexModel : PageModel
{
    // Fields.
    private readonly IConfiguration configuration;
    private readonly IEthernaInternalCreditClient creditClient;

    // Constructor.
    public IndexModel(
        IConfiguration configuration,
        IEthernaInternalCreditClient creditClient)
    {
        this.configuration = configuration;
        this.creditClient = creditClient;
    }

    // Properties.
    public XDaiBalance CreditBalance { get; set; }
    public bool IsUnlimitedCredit { get; set; }

    // Methods.
    public async Task OnGetAsync()
    {
        var credit = await creditClient.GetUserCreditAsync(configuration["SampleConfig:Address"]!);
        CreditBalance = credit.Balance;
        IsUnlimitedCredit = credit.IsUnlimited;
    }
}