module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    sass: {
      dev: {
        files: {
          'dist/css/screen.css': 'src/sass/screen.scss'
        },
        options: {
	      includePaths: [
            'src/govuk_template/assets/stylesheets',
            'src/govuk_frontend_toolkit/stylesheets'
          ],
          outputStyle: 'compressed'
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
    },

  });

  ;[
	'grunt-contrib-watch',
    'grunt-sass'
  ].forEach(function (task) {
    grunt.loadNpmTasks(task)
  })

  // Default task(s).
  grunt.registerTask('default', ['sass']);

};