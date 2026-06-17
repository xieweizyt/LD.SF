using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(typeof(LdSfDomainModule))]
public class LdSfApplicationContractsModule : AbpModule
{
}

