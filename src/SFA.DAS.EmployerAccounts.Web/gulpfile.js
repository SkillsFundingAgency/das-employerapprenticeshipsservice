"use strict";
const gulp = require('gulp');
const sass = require('gulp-sass');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const uglify = require('gulp-uglify');
const cleanCSS = require('gulp-clean-css');

const input = './src/sass/**/*.scss';
const output = './dist/css/';

let sassOptions;

sassOptions = {
  errLogToConsole: true,
    includePaths: [
    'src/govuk_template/assets/stylesheets',
    'src/govuk_frontend_toolkit/stylesheets'
  ],
};

gulp.task('watch', () => {
  gulp.watch(input, ['sass'])
    .on('change', (event) => {
      console.log(`File ${event.path} was ${event.type}, running tasks...`);
    });
});

gulp.task('sass', () => gulp
  .src(input)
  .pipe(sass(sassOptions))
  .pipe(gulp.dest(output)));


gulp.task('default', ['sass', 'watch']);