using System;

namespace OpenAi.Api
{
    public enum ENewEngineName
    {
        ada,
        babbage,
        content_filter_alpha_c4,
        content_filter_dev,
        curie,
        cursing_filter_v6,
        davinci,
        instruct_curie_beta,
        instruct_davinci_beta,
        rephraserDavinci,
        classifictaionDavinci
    }

    public static class NewUTEngineName
    {
        public static string GetEngineName(ENewEngineName name)
        {
            switch (name)
            {
                case ENewEngineName.ada:
                    return NewUTEngineNames.ada;
                case ENewEngineName.babbage:
                    return NewUTEngineNames.babbage;
                case ENewEngineName.content_filter_alpha_c4:
                    return NewUTEngineNames.content_filter_alpha_c4;
                case ENewEngineName.content_filter_dev:
                    return NewUTEngineNames.content_filter_dev;
                case ENewEngineName.curie:
                    return NewUTEngineNames.curie;
                case ENewEngineName.cursing_filter_v6:
                    return NewUTEngineNames.cursing_filter_v6;
                case ENewEngineName.davinci:
                    return NewUTEngineNames.davinci;
                case ENewEngineName.instruct_curie_beta:
                    return NewUTEngineNames.instruct_curie_beta;
                case ENewEngineName.instruct_davinci_beta:
                    return NewUTEngineNames.instruct_davinci_beta;
                case ENewEngineName.classifictaionDavinci:
                    return NewUTEngineNames.classification_davinci;
                case ENewEngineName.rephraserDavinci:
                    return NewUTEngineNames.rephrase_davinci;
            }

            throw new ArgumentException($"Invalid enum value provided when getting engine name. Value provided: {name}");
        }
    }
}
