const fetch = require('isomorphic-fetch');

const SiteConfig = {
    setupSite: function setupSite(siteName) {
        const themeUrl = `http://sitebuilder.intuitivesystems.co.uk/sites/${siteName}/instances/dev/entities/theme/en/default`;
        return fetch(themeUrl)
            .then(result => result.json())
            .then(theme => {
                const themeContent = JSON.parse(theme.Content);
                return this.createThemeVariables(themeContent);
            })
            .then(themeVariables => {
                const config = {
                    name: siteName,
                    themeVariables,
                };
                return config;
            });
    },
    createThemeVariables: function createThemeVariables(theme) {
        const fontSizeBase = 16;
        return new Promise((resolve) => {
            const variables = {
                // colours
                '$gray-dark': theme.Colours.GreyDark,
                '$gray': theme.Colours.Grey,
                '$gray-light': theme.Colours.GreyLight,
                '$gray-lighter': theme.Colours.GreyLighter,
                '$brand-primary': theme.Colours.BrandPrimary,
                '$brand-success': theme.Colours.BrandSuccess,
                '$brand-info': theme.Colours.BrandInfo,
                '$brand-warning': theme.Colours.BrandWarning,
                '$brand-danger': theme.Colours.BrandDanger,
                '$brand-inverse': theme.Colours.BrandInverse,

                // generic
                '$body-bg': theme.Generic.BodyBg,
                '$spacing-xs': `${theme.Generic.SpacingExtraSmall}px`,
                '$spacing-sm': `${theme.Generic.SpacingSmall}px`,
                '$spacing-md': `${theme.Generic.SpacingMedium}px`,
                '$spacing-lg': `${theme.Generic.SpacingLarge}px`,


                // typography
                '$header-font-stack': theme.Typography.HeaderFontStack,
                '$preamble-font-stack': theme.Typography.PreambleFontStack,
                '$body-font-stack': theme.Typography.BodyFontStack,
                '$link-font-stack': theme.Typography.LinkFontStack,

                '$header-primary-font-size': `${theme.Typography.HeaderPrimary.FontSize / fontSizeBase}rem`,
                '$header-primary-font-weight': theme.Typography.HeaderPrimary.FontWeight,
                '$header-primary-line-height': `${theme.Typography.HeaderPrimary.LineHeight / fontSizeBase}rem`,
                '$header-primary-color': theme.Typography.HeaderPrimary.Colour,
                '$header-primary-inverse-color': theme.Typography.HeaderPrimary.InverseColour,

                '$header-secondary-font-size': `${theme.Typography.HeaderSecondary.FontSize / fontSizeBase}rem`,
                '$header-secondary-font-weight': theme.Typography.HeaderSecondary.FontWeight,
                '$header-secondary-line-height': `${theme.Typography.HeaderSecondary.LineHeight / fontSizeBase}rem`,
                '$header-secondary-color': theme.Typography.HeaderSecondary.Colour,
                '$header-secondary-inverse-color': theme.Typography.HeaderSecondary.InverseColour,

                '$header-tertiary-font-size': `${theme.Typography.HeaderTertiary.FontSize / fontSizeBase}rem`,
                '$header-tertiary-font-weight': theme.Typography.HeaderTertiary.FontWeight,
                '$header-tertiary-line-height': `${theme.Typography.HeaderTertiary.LineHeight / fontSizeBase}rem`,
                '$header-tertiary-color': theme.Typography.HeaderTertiary.Colour,
                '$header-tertiary-inverse-color': theme.Typography.HeaderTertiary.InverseColour,

                '$preamble-font-size': `${theme.Typography.Preamble.FontSize / fontSizeBase}rem`,
                '$preamble-font-weight': theme.Typography.Preamble.FontWeight,
                '$preamble-line-height': `${theme.Typography.Preamble.LineHeight / fontSizeBase}rem`,
                '$preamble-color': theme.Typography.Preamble.Colour,
                '$preamble-inverse-color': theme.Typography.Preamble.InverseColour,

                '$body-font-size': `${theme.Typography.Body.FontSize / fontSizeBase}rem`,
                '$body-font-weight': theme.Typography.Body.FontWeight,
                '$body-line-height': `${theme.Typography.Body.LineHeight / fontSizeBase}rem`,
                '$body-color': theme.Typography.Body.Colour,
                '$body-inverse-color': theme.Typography.Body.InverseColour,

                '$body-font-size-xs': `${theme.Typography.BodySmall.FontSize / fontSizeBase}rem`,
                '$body-font-weight-xs': theme.Typography.BodySmall.FontWeight,
                '$body-line-height-xs': `${theme.Typography.BodySmall.LineHeight / fontSizeBase}rem`,
                '$body-color-xs': theme.Typography.BodySmall.Colour,
                '$body-inverse-color-xs': theme.Typography.BodySmall.InverseColour,

                '$link-font-size': `${theme.Typography.Link.FontSize / fontSizeBase}rem`,
                '$link-font-weight': theme.Typography.Link.FontWeight,
                '$link-line-height': `${theme.Typography.Link.LineHeight / fontSizeBase}rem`,
                '$link-color': theme.Typography.Link.Colour,
                '$link-decoration': theme.Typography.Link.TextDecoration,
                '$link-hover-color': theme.Typography.Link.HoverColour,
                '$link-hover-decoration': theme.Typography.Link.HoverTextDecoration,

                // buttons
                '$btn-default-color': theme.Buttons.Colours.Default.Colour,
                '$btn-default-bg': theme.Buttons.Colours.Default.BackgroundColour,
                '$btn-default-border': this.cssBorderValue(theme.Buttons.Colours.Default.BorderWidth,
                    theme.Buttons.Colours.Default.BorderColour),
                '$btn-default-hover-color': theme.Buttons.Colours.Default.HoverColour,
                '$btn-default-hover-bg': theme.Buttons.Colours.Default.HoverBackgroundColour,
                '$btn-default-hover-border': this.cssBorderValue(theme.Buttons.Colours.Default.BorderWidth,
                    theme.Buttons.Colours.Default.HoverBorderColour),

                '$btn-primary-color': theme.Buttons.Colours.Primary.Colour,
                '$btn-primary-bg': theme.Buttons.Colours.Primary.BackgroundColour,
                '$btn-primary-border': this.cssBorderValue(theme.Buttons.Colours.Primary.BorderWidth,
                    theme.Buttons.Colours.Primary.BorderColour),
                '$btn-primary-hover-color': theme.Buttons.Colours.Primary.HoverColour,
                '$btn-primary-hover-bg': theme.Buttons.Colours.Primary.HoverBackgroundColour,
                '$btn-primary-hover-border': this.cssBorderValue(theme.Buttons.Colours.Primary.BorderWidth,
                    theme.Buttons.Colours.Primary.HoverBorderColour),

                '$btn-inverse-color': theme.Buttons.Colours.Inverse.Colour,
                '$btn-inverse-bg': theme.Buttons.Colours.Inverse.BackgroundColour,
                '$btn-inverse-border': this.cssBorderValue(theme.Buttons.Colours.Inverse.BorderWidth,
                    theme.Buttons.Colours.Inverse.BorderColour),
                '$btn-inverse-hover-color': theme.Buttons.Colours.Inverse.HoverColour,
                '$btn-inverse-hover-bg': theme.Buttons.Colours.Inverse.HoverBackgroundColour,
                '$btn-inverse-hover-border': this.cssBorderValue(theme.Buttons.Colours.Inverse.BorderWidth,
                    theme.Buttons.Colours.Inverse.HoverBorderColour),

                '$btn-height-xs': `${theme.Buttons.Sizes.ButtonExtraSmall.Height}px`,
                '$btn-height-sm': `${theme.Buttons.Sizes.ButtonSmall.Height}px`,
                '$btn-height-md': `${theme.Buttons.Sizes.ButtonMedium.Height}px`,
                '$btn-height-lg': `${theme.Buttons.Sizes.ButtonLarge.Height}px`,

                '$btn-padding-x-xs': `${theme.Buttons.Sizes.ButtonExtraSmall.PaddingX}px`,
                '$btn-padding-x-sm': `${theme.Buttons.Sizes.ButtonSmall.PaddingX}px`,
                '$btn-padding-x-md': `${theme.Buttons.Sizes.ButtonMedium.PaddingX}px`,
                '$btn-padding-x-lg': `${theme.Buttons.Sizes.ButtonLarge.PaddingX}px`,

                '$btn-font-size-xs': `${theme.Buttons.Sizes.ButtonExtraSmall.FontSize}px`,
                '$btn-font-size-sm': `${theme.Buttons.Sizes.ButtonSmall.FontSize}px`,
                '$btn-font-size-md': `${theme.Buttons.Sizes.ButtonMedium.FontSize}px`,
                '$btn-font-size-lg': `${theme.Buttons.Sizes.ButtonLarge.FontSize}px`,

                '$btn-font-weight-xs': `${theme.Buttons.Sizes.ButtonExtraSmall.FontWeight}`,
                '$btn-font-weight-sm': `${theme.Buttons.Sizes.ButtonSmall.FontWeight}`,
                '$btn-font-weight-md': `${theme.Buttons.Sizes.ButtonMedium.FontWeight}`,
                '$btn-font-weight-lg': `${theme.Buttons.Sizes.ButtonLarge.FontWeight}`,

                '$btn-border-radius-xs': `${theme.Buttons.Sizes.ButtonExtraSmall.BorderRadius}px`,
                '$btn-border-radius-sm': `${theme.Buttons.Sizes.ButtonSmall.BorderRadius}px`,
                '$btn-border-radius-md': `${theme.Buttons.Sizes.ButtonMedium.BorderRadius}px`,
                '$btn-border-radius-lg': `${theme.Buttons.Sizes.ButtonLarge.BorderRadius}px`,
            };
            let themeVariables = '';
            Object.keys(variables)
                .map(key => {
                    themeVariables = `${themeVariables}${key}:${variables[key]};`;
                });
            resolve(themeVariables);
        });
    },
    cssBorderValue: function cssBorderValue(width, colour) {
        let borderValue = 'none';
        if (width !== 0) {
            borderValue = `${width}px solid ${colour}`;
        }
        return borderValue;
    }
};

module.exports = SiteConfig;
