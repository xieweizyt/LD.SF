using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(
    typeof(LdSfApplicationContractsModule),
    typeof(Volo.Abp.Application.AbpDddApplicationModule))]
public class LdSfApplicationModule : AbpModule
{
}
