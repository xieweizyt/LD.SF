using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(
    typeof(LdSfDomainModule),
    typeof(Volo.Abp.Application.AbpDddApplicationContractsModule))]
public class LdSfApplicationContractsModule : AbpModule
{
}
