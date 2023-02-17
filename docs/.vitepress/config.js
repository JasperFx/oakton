module.exports = {
    title: 'Oakton',
    description: 'Add Robust Command Line Options to .Net Applications',
    head: [],
    base: '/oakton/',
    themeConfig: {
        logo: null,
        repo: 'JasperFx/oakton',
        docsDir: 'docs',
        docsBranch: 'master',
        editLinks: true,
        editLinkText: 'Suggest changes to this page',
        base: '/oakton/',
        nav: [
            { text: 'Guide', link: '/guide/' },
            { text: 'Discord | Join Chat', link: 'https://discord.gg/WMxrvegf8H' }
        ],

        algolia: {
            appId: '5G434AGRPI',
            apiKey: 'daed9f557ebf95072747f226f6117220',
            indexName: 'oakton_index'
        },

        sidebar: {
            '/': 
            [
                {
                    text: 'Getting Started',
                    collapsible: false,
                    collapsed: false,
                    items: tableOfContents()
                }
            ]
        }
    },
    markdown: {
        linkify: false
    }
}

function tableOfContents() {
    return [
      {text: 'What is Oakton', link: '/guide/'},
      {text: "Commands", link: '/guide/commands'},
      {
        text: "Integration with IHost",
        link: '/guide/host/',
        collapsible: true,
        collapsed: true,
        items: [
          {text: "Integration with IHost", link: '/guide/host'},
          {text: "Improved \"Run\" Command", link: '/guide/host/run'},
          {text: "Environment Checks", link: '/guide/host/environment'},
          {text: "Writing Extension Commands", link: '/guide/host/extensions'},
          {text: "The \"describe\" command", link: '/guide/host/describe'},
          {text: "Stateful Resources", link: '/guide/host/resources'}
        ]
      },
      {text: "Bootstrapping with CommandExecutor", link: '/guide/bootstrapping'},
      {text: "Parsing Arguments and Optional Flags", link: '/guide/parsing'},
      {text: "Help Text", link: '/guide/help'},
      {text: "\"Opts\" Files", link: '/guide/opts'},
      {text: "Command Assembly Discovery", link: '/guide/discovery'},
    ]
}
