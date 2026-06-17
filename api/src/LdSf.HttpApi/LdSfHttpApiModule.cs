using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(
    typeof(LdSfApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class LdSfHttpApiModule : AbpModule
{
}

