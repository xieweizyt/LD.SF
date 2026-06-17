using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(
    typeof(LdSfApplicationContractsModule),
    typeof(AbpAutoMapperModule))]
public class LdSfApplicationModule : AbpModule
{
}

