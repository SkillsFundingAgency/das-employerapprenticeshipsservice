const sass = require('node-sass');

module.exports = grunt => {
    require('load-grunt-tasks')(grunt);

    grunt.initConfig({
        svg_sprite: {
            svgsprites: {
                src: ['src/svg/*.svg'],
                dest: 'Views',
                options: {
                    shape: {
                        dimension: {
                            maxWidth: 40,
                            maxHeight: 40
                        },
                        id: {
                            generator: function(name, file) {
                                return 'icon-' + file.stem;
                            }
                        },
                        spacing: {
                            padding: 5
                        }
                    },
                    mode: {
                        symbol: {
                            dest: 'Shared',
                            sprite: '_SVGSprite.cshtml'
                        }
                    }
                }
            }
        },
        sass: {
            dev: {
                files: {
                    'dist/css/screen.css': 'src/sass/screen.scss',
                    'dist/css/screen-ie6.css': 'src/sass/screen-ie6.scss',
                    'dist/css/screen-ie7.css': 'src/sass/screen-ie7.scss',
                    'dist/css/screen-ie8.css': 'src/sass/screen-ie8.scss'
                },
                options: {
                    implementation: sass,
                    includePaths: [
                        'src/govuk_template/assets/stylesheets',
                        'src/govuk_frontend_toolkit/stylesheets'
                    ],
                    outputStyle: 'expanded',
                    noCache: true
                }
            }
        },
        // Watches styles and specs for changes
        watch: {
            css: {
                files: ['src/sass/*.scss'],
                tasks: ['sass'],
                options: { nospawn: true }
            }
        }
    });

    ['grunt-contrib-watch', 'grunt-svg-sprite', 'grunt-sass'].forEach(function(task) {
        grunt.loadNpmTasks(task);
    });

    // Default task(s).
    grunt.registerTask('svg', ['svg_sprite']);
    grunt.registerTask('default', ['sass', 'watch']);
};