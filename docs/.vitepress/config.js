module.exports = {
    title: 'Oakton',
    description: 'Add Robust Command Line Options to .Net Applications',
    head: [],
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
            { text: 'Gitter | Join Chat', link: 'https://gitter.im/JasperFx/oakton?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge' }
        ],

        algolia: {
            appId: '2V5OM390DF',
            apiKey: '674cd4f3e6b6ebe232a980c7cc5a0270',
            indexName: 'oakton_index'
        },

        sidebar: [
            {
                text: 'Getting Started',
                link: '/guide/',
                children: tableOfContents()
            }
        ]
    },
    markdown: {
        linkify: false
    }
}

function tableOfContents() {
    return [
      {text: "Commands", link: '/guide/commands'},
      {
        text: "Integration with IHost",
        link: '/guide/host/',
        children: [
          {text: "Improved \"Run\" Command", link: '/guide/host/run'},
          {text: "Environment Checks", link: '/guide/host/environment'},
          {text: "Writing Extension Commands", link: '/guide/host/extensions'},
          {text: "The \"describe\" command", link: '/guide/host/describe'}


        ]
      },
      {text: "Bootstrapping with CommandExecutor", link: '/guide/bootstrapping'},
      {text: "Parsing Arguments and Optional Flags", link: '/guide/parsing'},
      {text: "Help Text", link: '/guide/help'},
      {text: "\"Opts\" Files", link: '/guide/opts'},
      {text: "Command Assembly Discovery", link: '/guide/discovery'},


    ]
}
