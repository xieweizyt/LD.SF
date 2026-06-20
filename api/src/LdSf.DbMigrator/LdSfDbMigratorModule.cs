using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(
    typeof(LdSfEntityFrameworkCoreModule))]
public class LdSfDbMigratorModule : AbpModule
{
}
