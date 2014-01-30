(function (cloudStack) {
  cloudStack.plugins.appcore = function(plugin) {
    plugin.ui.addSection({
      id: 'appcore',
      title: 'Appcore',
      preFilter: function(args) {
        return isAdmin();
      },
      show: function() {
        return $('<div>').html('Content will go here');
      }
    });
  };
}(cloudStack));